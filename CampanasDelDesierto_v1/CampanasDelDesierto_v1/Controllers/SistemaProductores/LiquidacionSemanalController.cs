using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;

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
        public ActionResult Create(int? id, int? temporada)
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

            LiquidacionSemanal mov = prepararVistaCrear(productor);
            mov.Productor = productor;
            mov.introducirMovimientoEnPeriodo(temporada);

            return View(mov);
        }

        private LiquidacionSemanal prepararVistaCrear(Productor productor)
        {
            ViewBag.balanceActual = productor.balanceActual;
            LiquidacionSemanal mov = new LiquidacionSemanal();
            mov.idProductor = productor.idProductor;
            return mov;
        }

        // POST: EmisionDeCheques/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,"+
            "TemporadaDeCosechaID,cheque,garantiaLimpieza,retenciones.garantiaLimpieza")]
            LiquidacionSemanal emisionDeCheque, LiquidacionSemanal.VMRetenciones retenciones)
        {
            if (ModelState.IsValid)
            {
                //Se crean los registros de retenciones
                Deduccion limpieza = new Deduccion(emisionDeCheque, retenciones.garantiaLimpieza,
                    Deduccion.TipoDeduccion.SANIDAD);
                int numReg = 0;

                //Ajuste de movimiento para entrar dentro de la lista de balances
                emisionDeCheque.ajustarMovimiento();

                //Solamente se agregara deposito para limpieza si fue introducida una cantidad mayor a 0
                if (limpieza.montoMovimiento != 0)
                {
                    //Se le establecen fechas con horas diferentes para mantener la integridad del balance
                    //Las retenciones apareceran primero
                    limpieza.ajustarMovimiento();

                    limpieza.fechaMovimiento = emisionDeCheque.fechaMovimiento.AddSeconds(-1);

                    db.Deducciones.Add(limpieza);
                    numReg = db.SaveChanges();
                    if (numReg > 0)
                        numReg = introducirMovimientoAlBalance(limpieza);
                }

                db.LiquidacionesSemanales.Add(emisionDeCheque);
                numReg = db.SaveChanges();

                if (numReg > 0)
                {
                    //numReg = introducirMovimientoAlBalance(emisionDeCheque);
                    return RedirectToAction("Details", "Productores", new { id = emisionDeCheque.idProductor,
                        temporada = emisionDeCheque.TemporadaDeCosechaID });
                }
            }

            Productor productor = db.Productores.Find(emisionDeCheque.idProductor);
            ViewBag.productor = productor;
            prepararVistaCrear(productor);

            return View(emisionDeCheque);
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
        public ActionResult Edit(int? id)
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
            
            return View("Create",emisionDeCheque);
        }

        // POST: EmisionDeCheques/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,"+
            "idProductor,TemporadaDeCosechaID,cheque")]
            LiquidacionSemanal emisionDeCheque)
        {
            //Ajuste de movimiento para entrar dentro de la lista de balances
            emisionDeCheque.ajustarMovimiento();
            int numReg = 0;
            if (ModelState.IsValid)
            {
                db.Entry(emisionDeCheque).State = EntityState.Modified;
                numReg = db.SaveChanges();
                if (numReg > 0)
                {
                    numReg = introducirMovimientoAlBalance(emisionDeCheque);
                    return RedirectToAction("Details", "Productores", new
                    {
                        id = emisionDeCheque.idProductor,
                        temporada = emisionDeCheque.TemporadaDeCosechaID
                    });
                }
            }

            return View(emisionDeCheque);
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
