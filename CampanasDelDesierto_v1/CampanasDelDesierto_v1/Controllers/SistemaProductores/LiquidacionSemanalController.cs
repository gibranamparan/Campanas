using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;
using CampanasDelDesierto_v1.HerramientasGenerales;

namespace CampanasDelDesierto_v1.Controllers
{
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class LiquidacionSemanalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EmisionDeCheques
        public ActionResult Index()
        {
            var movimientosFinancieros = db.MovimientosFinancieros.Include(e => e.Productor).Include(e => e.temporadaDeCosecha);
            return View(movimientosFinancieros.ToList());
        }

        // GET: EmisionDeCheques/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiquidacionSemanal emisionDeCheque = db.LiquidacionesSemanales.Find(id);
            if (emisionDeCheque == null)
            {
                return HttpNotFound();
            }


            return View(emisionDeCheque);
        }

        // GET: EmisionDeCheques/Create
        public ActionResult Create(int? id, int? temporada, TimePeriod semanaLiquidada, int semana=0)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Productor productor = db.Productores.Find(id);
            if (productor == null)
            {
                return HttpNotFound();
            }

            LiquidacionSemanal mov = prepararVistaCrear(productor, semanaLiquidada);
            mov.introducirMovimientoEnPeriodo(temporada);

            if (semana == 0)
            {
                //Se determina el numero de semana actual de registro segun el consecutivo de liquidaciones registradas en la base de datos
                var liquidaciones = db.LiquidacionesSemanales.Where(liq => liq.TemporadaDeCosechaID == temporada);
                int noSemana = 0;
                if (liquidaciones.Count() > 0)
                    noSemana = liquidaciones.OrderBy(liq => liq.semana).FirstOrDefault().semana;
                mov.semana = noSemana + 1;
            }
            else
                mov.semana = semana;

            return View(mov);
        }

        /// <summary>
        /// Rutina para preparar vistas de edicion y creacion
        /// </summary>
        /// <param name="productor"></param>
        /// <param name="semanaLiquidada"></param>
        /// <returns></returns>
        private LiquidacionSemanal prepararVistaCrear(Productor productor, TimePeriod semanaLiquidada)
        {
            ViewBag.balanceActual = productor.balanceActual;
            LiquidacionSemanal mov = new LiquidacionSemanal();
            mov.idProductor = productor.idProductor;
            mov.Productor = productor;

            if (semanaLiquidada.isNotDefaultInstance())
            {
                //El limite final del periodo semanal se configura para cubrir la totalidad del ultimo dia0
                semanaLiquidada.endDate = semanaLiquidada.endDate.AddHours(24).AddSeconds(-1);
                //Se asocia al nuevo registro de liquidacion semanal
                mov.semanaLiquidada = semanaLiquidada;
            }
            return mov;
        }

        // POST: EmisionDeCheques/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,"+
            "TemporadaDeCosechaID,cheque,garantiaLimpieza,retenciones.garantiaLimpieza")]
            LiquidacionSemanal emisionDeCheque, LiquidacionSemanal.VMRetenciones retenciones, int[] ingresosDeCosechaID)
        {
            if (ModelState.IsValid && ingresosDeCosechaID.Count()>0)
            {
                int numReg = 0;
                List<Retencion> arrRetenciones = vmRetencionesToArray(retenciones, emisionDeCheque);

                //Ajuste de movimiento para entrar dentro de la lista de balances
                emisionDeCheque.ajustarMovimiento();

                //Si se agrego al menos una retencion a la lista, se marca para ser guardada
                if (arrRetenciones.Count() > 0)
                    emisionDeCheque.retenciones = arrRetenciones;

                //Si se registro un abono, se instancia
                PrestamoYAbonoCapital abono = new PrestamoYAbonoCapital();
                if (retenciones.abonoAnticipos > 0)
                {
                    //Se crea un nuevo abono como retencion de esta liquidacion
                    abono = PrestamoYAbonoCapital.nuevaRentecionAbono(emisionDeCheque, retenciones.abonoAnticipos);

                    //Se marca para guardar
                    emisionDeCheque.abonoAnticipo = abono;
                }

                db.LiquidacionesSemanales.Add(emisionDeCheque);
                numReg = db.SaveChanges();

                if (numReg > 0)
                {
                    //Asociar ingresos de cosecha a emision de cheque
                    foreach(int ingID in ingresosDeCosechaID)
                    {
                        var ingreso = db.PagosPorProductos.Find(ingID);
                        ingreso.liquidacionDeCosechaID = emisionDeCheque.idMovimiento;
                        db.Entry(ingreso).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                    //Se ajusta el abalance si se registro una retencion por abono
                    if (abono.idMovimiento > 0) { 
                        //Se calcula el movimiento anterior al que se esta registrando
                        var prod = db.Productores.Find(abono.idProductor);
                        MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(abono.fechaMovimiento);

                        //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                        prod.ajustarBalances(ultimoMovimiento, db);
                    }
                    
                    return RedirectToAction("Details", "Productores", new { id = emisionDeCheque.idProductor,
                        temporada = emisionDeCheque.TemporadaDeCosechaID });
                }
            }

            Productor productor = db.Productores.Find(emisionDeCheque.idProductor);
            LiquidacionSemanal mov = prepararVistaCrear(productor, emisionDeCheque.semanaLiquidada);
            mov.introducirMovimientoEnPeriodo(emisionDeCheque.TemporadaDeCosechaID);

            return View(emisionDeCheque);
        }

        /// <summary>
        /// Representa una arreglo de todas las retenciones ingresadas en la forma de la Liquidacion Semanal
        /// </summary>
        /// <param name="retenciones">Modelo virtual con las cantidades ingresadas en la forma para cada retencion</param>
        /// <param name="emisionDeCheque">Instancia del registro de liquidacion semanal asociado a las retenciones</param>
        /// <returns></returns>
        private List<Retencion> vmRetencionesToArray(LiquidacionSemanal.VMRetenciones retenciones, 
            LiquidacionSemanal emisionDeCheque)
        {
            //Se crean los registros de retenciones
            Retencion garantiaLimpieza = new Retencion(emisionDeCheque, retenciones.garantiaLimpieza,
                Retencion.TipoRetencion.SANIDAD);
            Retencion retencionEjidal = new Retencion(emisionDeCheque, retenciones.retencionEjidal,
                Retencion.TipoRetencion.EJIDAL);
            Retencion retencionOtro = new Retencion(emisionDeCheque, retenciones.retencionOtro,
                Retencion.TipoRetencion.OTRO);

            List<Retencion> res = new List<Retencion>();
            //Se le establecen fechas con horas diferentes para mantener la integridad del balance
            //Las retenciones apareceran primero
            garantiaLimpieza.fechaMovimiento = emisionDeCheque.fechaMovimiento.AddSeconds(-1);
            retencionEjidal.fechaMovimiento = garantiaLimpieza.fechaMovimiento.AddSeconds(-1);
            retencionOtro.fechaMovimiento = retencionEjidal.fechaMovimiento.AddSeconds(-1);

            //Se valida si fue introducido un monto para cada retencion
            if(garantiaLimpieza.montoMovimiento<0)
                res.Add(garantiaLimpieza);
            if (retencionEjidal.montoMovimiento < 0)
                res.Add(retencionEjidal);
            if (retencionOtro.montoMovimiento < 0)
                res.Add(retencionOtro);

            return res;
        }

        private int introducirMovimientoAlBalance(MovimientoFinanciero mov)
        {
            Productor productor = db.Productores.Find(mov.idProductor);

            //Se calcula el movimiento anterior al que se esta registrando
            var ultimoMovimiento = productor.getUltimoMovimiento(mov.fechaMovimiento);

            //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
            return productor.ajustarBalances(ultimoMovimiento, db);
        }

        // GET: EmisionDeCheques/Edit/5
        public ActionResult Edit(int? id, TimePeriod semanaLiquidada)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LiquidacionSemanal mov = db.LiquidacionesSemanales.Find(id);
            if (mov == null)
            {
                return HttpNotFound();
            }

            prepararVistaCrear(mov.Productor,semanaLiquidada);

            return View("Create",mov);
        }

        // POST: EmisionDeCheques/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,"+
            "TemporadaDeCosechaID,cheque,garantiaLimpieza,retenciones.garantiaLimpieza")]
            LiquidacionSemanal emisionDeCheque, LiquidacionSemanal.VMRetenciones retenciones)
        {
            if (ModelState.IsValid)
            {
                int numReg = 0;
                List<Retencion> arrRetenciones = vmRetencionesToArray(retenciones, emisionDeCheque);

                //Ajuste de movimiento para entrar dentro de la lista de balances
                emisionDeCheque.ajustarMovimiento();

                //Si se agrego al menos una retencion a la lista, se marca para ser guardada
                if (arrRetenciones.Count() > 0)
                    emisionDeCheque.retenciones = arrRetenciones;

                //Si se registro un abono, se instancia
                PrestamoYAbonoCapital abono = new PrestamoYAbonoCapital();
                if (retenciones.abonoAnticipos > 0)
                {
                    //Se crea un nuevo abono como retencion de esta liquidacion
                    abono = PrestamoYAbonoCapital.nuevaRentecionAbono(emisionDeCheque, retenciones.abonoAnticipos);
                    //Se marca para guardar
                    emisionDeCheque.abonoAnticipo = abono;
                }

                db.LiquidacionesSemanales.Add(emisionDeCheque);
                numReg = db.SaveChanges();

                if (numReg > 0)
                {
                    if (abono.idMovimiento > 0)
                    {
                        //Se calcula el movimiento anterior al que se esta registrando
                        var prod = db.Productores.Find(abono.idProductor);
                        MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(abono.fechaMovimiento);

                        //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                        prod.ajustarBalances(ultimoMovimiento, db);
                    }

                    //numReg = introducirMovimientoAlBalance(emisionDeCheque);
                    return RedirectToAction("Details", "Productores", new
                    {
                        id = emisionDeCheque.idProductor,
                        temporada = emisionDeCheque.TemporadaDeCosechaID
                    });
                }
            }

            Productor productor = db.Productores.Find(emisionDeCheque.idProductor);
            LiquidacionSemanal mov = prepararVistaCrear(productor, emisionDeCheque.semanaLiquidada);
            mov.introducirMovimientoEnPeriodo(emisionDeCheque.TemporadaDeCosechaID);

            return View("Create", mov);
        }

        // GET: EmisionDeCheques/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiquidacionSemanal emisionDeCheque = db.LiquidacionesSemanales.Find(id);
            if (emisionDeCheque == null)
            {
                return HttpNotFound();
            }
            return View(emisionDeCheque);
        }

        // POST: EmisionDeCheques/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LiquidacionSemanal emisionDeCheque = db.LiquidacionesSemanales.Find(id);
            db.MovimientosFinancieros.Remove(emisionDeCheque);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult PrintCheque(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiquidacionSemanal emisionDeCheque = db.LiquidacionesSemanales.Find(id);
            if (emisionDeCheque == null)
            {
                return HttpNotFound();
            }
            return View("Cheque", emisionDeCheque);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
