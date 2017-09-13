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
        private const string strBindFields = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor," +
            "TemporadaDeCosechaID,cheque,,semana,semanaLiquidada.startDate,semanaLiquidada.endDate," +
            "abonoAnticipoID,abonoArbolesID,precioDelDolarEnLiquidacion";
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            LiquidacionSemanal emisionDeCheque = db.LiquidacionesSemanales.Find(id);
            if (emisionDeCheque == null)
                return HttpNotFound();

            return View(emisionDeCheque);
        }

        /// <summary>
        /// Rutina para preparar vistas de edicion y creacion
        /// </summary>
        /// <param name="productor"></param>
        /// <param name="semanaLiquidada"></param>
        /// <returns></returns>
        private LiquidacionSemanal prepararVistaCrear(Productor productor, TimePeriod semanaLiquidada, TemporadaDeCosecha temporada, decimal precioDelDolarEnLiquidacion)
        {
            LiquidacionSemanal mov = new LiquidacionSemanal();
            mov.idProductor = productor.idProductor;
            mov.Productor = productor;
            mov.precioDelDolarEnLiquidacion = precioDelDolarEnLiquidacion;
            ViewBag.balanceActual = productor.balanceDeAnticiposEnFecha(mov.fechaMovimiento);
            ViewBag.balanceActualArboles = productor.balanceArbolesEnFecha(mov.fechaMovimiento);

            if (semanaLiquidada.isNotDefaultInstance())
                mov.semanaLiquidada = semanaLiquidada; //Se asocia al nuevo registro de liquidacion semanal

            mov.introducirMovimientoEnPeriodo(temporada);
            ViewBag.reporteMovimientos = productor.generarReporteAnticiposConIntereses(mov.fechaMovimiento, mov.temporadaDeCosecha,db);

            return mov;
        }

        /// <summary>
        /// Rutina para preparar vistas de edicion y creacion
        /// </summary>
        /// <param name="productor"></param>
        /// <param name="semanaLiquidada"></param>
        /// <returns></returns>
        private void prepararVistaEditar(ref LiquidacionSemanal mov, TimePeriod semanaLiquidada)
        {
            ViewBag.balanceActual = mov.Productor.balanceActual;
            ViewBag.balanceActualArboles = mov.Productor.balanceActualArboles;
            mov.idProductor = mov.Productor.idProductor;
            mov.Productor = mov.Productor;

            List<object> opcionesTipoCapital = PrestamoYAbonoCapital.getTipoMovimientoCapitalArray(true);
            ViewBag.opcionesTipoCapital = opcionesTipoCapital;

            if (semanaLiquidada != null && semanaLiquidada.isNotDefaultInstance())
            {
                //El limite final del periodo semanal se configura para cubrir la totalidad del ultimo dia0
                semanaLiquidada.endDate = semanaLiquidada.endDate.AddHours(24).AddSeconds(-1);
                //Se asocia al nuevo registro de liquidacion semanal
                mov.semanaLiquidada = semanaLiquidada;
            }
            ViewBag.reporteMovimientos = mov.Productor.generarReporteAnticiposConIntereses(mov.fechaMovimiento, mov.temporadaDeCosecha, db);
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
            Retencion retencionAbonoAnticipo = new Retencion(emisionDeCheque, retenciones.abonoAnticipos,
                Retencion.TipoRetencion.ABONO_ANTICIPO);
            Retencion retencionAbonoArboles = new Retencion(emisionDeCheque, retenciones.abonoArboles,
                Retencion.TipoRetencion.ABONO_ARBOLES);

            List<Retencion> res = new List<Retencion>();
            //Se le establecen fechas con horas diferentes para mantener la integridad del balance
            //Las retenciones apareceran primero
            garantiaLimpieza.fechaMovimiento = emisionDeCheque.fechaMovimiento.AddSeconds(-1);
            retencionEjidal.fechaMovimiento = garantiaLimpieza.fechaMovimiento.AddSeconds(-1);
            retencionOtro.fechaMovimiento = retencionEjidal.fechaMovimiento.AddSeconds(-1);
            retencionAbonoAnticipo.fechaMovimiento = retencionOtro.fechaMovimiento.AddSeconds(-1);
            retencionAbonoArboles.fechaMovimiento = retencionAbonoAnticipo.fechaMovimiento.AddSeconds(-1);

            //Se valida si fue introducido un monto para cada retencion
            if (garantiaLimpieza.montoMovimiento < 0)
                res.Add(garantiaLimpieza);
            if (retencionEjidal.montoMovimiento < 0)
                res.Add(retencionEjidal);
            if (retencionOtro.montoMovimiento < 0)
                res.Add(retencionOtro);
            if (retencionAbonoAnticipo.montoMovimiento < 0)
                res.Add(retencionAbonoAnticipo);
            if (retencionAbonoArboles.montoMovimiento < 0)
                res.Add(retencionAbonoArboles);

            return res;
        }

        // GET: EmisionDeCheques/Create
        public ActionResult Create(int? id, int? temporada, TimePeriod semanaLiquidada, int semana=0, decimal precioDelDolarEnLiquidacion = 0)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Productor productor = db.Productores.Find(id);
            if (productor == null)
                return HttpNotFound();

            TemporadaDeCosecha temporadaCosecha = db.TemporadaDeCosechas.Find(temporada);
            LiquidacionSemanal mov = prepararVistaCrear(productor, semanaLiquidada, temporadaCosecha, precioDelDolarEnLiquidacion);

            if (semana == 0)
            {
                //Se determina el numero de semana actual de registro segun el consecutivo de liquidaciones registradas en la base de datos
                var liquidaciones = db.LiquidacionesSemanales.Where(liq => liq.TemporadaDeCosechaID == temporada);
                int noSemana = 0;
                if (liquidaciones.Count() > 0)
                    noSemana = liquidaciones.OrderByDescending(liq => liq.semana).FirstOrDefault().semana;
                mov.semana = noSemana + 1;
            }
            else
                mov.semana = semana;

            return View("Form_Liquidacion",mov);
        }

        // POST: EmisionDeCheques/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = LiquidacionSemanalController.strBindFields)]
            LiquidacionSemanal emisionDeCheque, LiquidacionSemanal.VMRetenciones retenciones,
            TimePeriod semanaLiquidada, int[] ingresosDeCosechaID)
        {
            if (ModelState.IsValid && ingresosDeCosechaID.Count()>0)
            {
                int numReg = 0;
                //Ajuste de movimiento para entrar dentro de la lista de balances
                emisionDeCheque.ajustarMovimiento();

                //Toma el input de retenciones entrante y lo transforma en registros de retenciones
                //para ser mostrados en el balance corresspondiente
                List<Retencion> arrRetenciones = vmRetencionesToArray(retenciones, emisionDeCheque);
                emisionDeCheque.semanaLiquidada = semanaLiquidada;
                
                //Si se agrego al menos una retencion a la lista, se marca para ser guardada
                if (arrRetenciones.Count() > 0)
                    emisionDeCheque.retenciones = arrRetenciones;

                db.LiquidacionesSemanales.Add(emisionDeCheque);
                numReg = db.SaveChanges();

                if (numReg > 0)
                {
                    PagoPorProducto ingreso = new PagoPorProducto();
                    //Asociar ingresos de cosecha a emision de cheque
                    foreach(int ingID in ingresosDeCosechaID)
                    {
                        ingreso = db.PagosPorProductos.Find(ingID);
                        ingreso.liquidacionDeCosechaID = emisionDeCheque.idMovimiento;
                        db.Entry(ingreso).State = EntityState.Modified;
                    }

                    //Si se registro un abono, se instancia
                    PrestamoYAbonoCapital abonoAnticipo = new PrestamoYAbonoCapital();
                    PrestamoYAbonoCapital abonoArboles = new PrestamoYAbonoCapital();
                    if (retenciones.abonoAnticipos > 0)
                    {
                        //Se crea un nuevo abono como retencion de esta liquidacion
                        abonoAnticipo = PrestamoYAbonoCapital.nuevaRentecionAbono(emisionDeCheque,
                            retenciones.abonoAnticipos, PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO);
                        //Se marca para guardar
                        emisionDeCheque.abonoAnticipo = abonoAnticipo;
                    }
                    if (retenciones.abonoArboles > 0)
                    {
                        //Se crea un nuevo abono como retencion de esta liquidacion
                        abonoArboles = PrestamoYAbonoCapital.nuevaRentecionAbono(emisionDeCheque,
                            retenciones.abonoArboles, PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO_ARBOLES);
                        //Se marca para guardar
                        emisionDeCheque.abonoArboles = abonoArboles;
                    }

                    //AJUSTE DE LA FECHA DE LA LIQUIDACION PARA ORDERNAR CON LAS RETENCIONES EN LA LISTA DE MOVIMIENTOS
                    //Se crea una nueva fecha igual en anio, mes y dia a la original del cheque
                    DateTime fechaMovimiento = new DateTime(emisionDeCheque.fechaMovimiento.Year,
                        emisionDeCheque.fechaMovimiento.Month, emisionDeCheque.fechaMovimiento.Day);
                    //Se establece el horario del cheque igual al del ultimo movimiento agregando 1ms para ajustar el orden
                    //dentro de la tabla de balances
                    fechaMovimiento = fechaMovimiento.AddMilliseconds(ingreso.fechaMovimiento.TimeOfDay.TotalMilliseconds + 1);
                    emisionDeCheque.fechaMovimiento = fechaMovimiento;

                    //Se guardan cambios de ajuste de hora del movimiento y los pagos
                    db.Entry(emisionDeCheque).State = EntityState.Modified;
                    numReg = db.SaveChanges();

                    //Se ajusta el abalance si se registro una retencion por abono de ANTICIPOS
                    if (abonoAnticipo.idMovimiento > 0)
                    {
                        //Se calcula el movimiento anterior al que se esta registrando
                        var prod = db.Productores.Find(abonoAnticipo.idProductor);
                        MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(abonoAnticipo.fechaMovimiento, abonoAnticipo.tipoDeBalance);
                        
                        //Se asocia el nuevo abono a los prestamos existentes en el balance
                        if (abonoAnticipo.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS)
                            prod.asociarAbonosConPrestamos(db, abonoAnticipo);

                        //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                        prod.ajustarBalances(ultimoMovimiento, db, abonoAnticipo.tipoDeBalance);
                    }

                    //Se ajusta el abalance si se registro una retencion por abono de ARBOLES
                    if (abonoArboles.idMovimiento > 0)
                    {
                        //Se calcula el movimiento anterior al que se esta registrando
                        var prod = db.Productores.Find(abonoArboles.idProductor);
                        MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(abonoArboles.fechaMovimiento, abonoArboles.tipoDeBalance);

                        //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                        prod.ajustarBalances(ultimoMovimiento, db, abonoArboles.tipoDeBalance);
                    }

                    //Se redirecciona para visualizar el reporte e imprimirlo una vez creado
                    return RedirectToAction("ReporteLiquidacionSemanal", new {
                        id = emisionDeCheque.idMovimiento,
                    });
                }
            }

            Productor productor = db.Productores.Find(emisionDeCheque.idProductor);
            TemporadaDeCosecha temporadaCosecha = db.TemporadaDeCosechas.Find(emisionDeCheque.TemporadaDeCosechaID);
            LiquidacionSemanal mov = prepararVistaCrear(productor, emisionDeCheque.semanaLiquidada, temporadaCosecha, emisionDeCheque.precioDelDolarEnLiquidacion);

            return View("Form_Liquidacion", emisionDeCheque);
        }
        
        // GET: EmisionDeCheques/Edit/5
        public ActionResult Edit(int? id, TimePeriod semanaLiquidada, bool actualizar=false)
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

            prepararVistaEditar(ref mov,semanaLiquidada);
            ViewBag.actualizar = actualizar;

            return View("Form_Liquidacion", mov);
        }

        // POST: EmisionDeCheques/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = LiquidacionSemanalController.strBindFields)]
            LiquidacionSemanal emisionDeCheque, TimePeriod semanaLiquidada,
            LiquidacionSemanal.VMRetenciones retenciones, int[] ingresosDeCosechaID)
        {
            if (ModelState.IsValid)
            {
                bool asociarEnEdicion = false;
                int numReg = 0;

                //Ajuste de movimiento para entrar dentro de la lista de balances
                emisionDeCheque.semanaLiquidada = semanaLiquidada;
                emisionDeCheque.ajustarMovimiento();

                //Se toma las retenciones que ya habian sido guardadas
                List<Retencion> retencionesPrevias = db.Retenciones
                    .Where(mov => mov.liquidacionSemanalID == emisionDeCheque.idMovimiento).ToList();
                //Se toman las nuevas retenciones
                List<Retencion> arrRetenciones = vmRetencionesToArray(retenciones, emisionDeCheque);

                //Se genera arreglo de tipos de retenciones para checar ediciones
                var tiposRetencionesArray = Enum.GetValues(typeof(Retencion.TipoRetencion))
                    .Cast<Retencion.TipoRetencion>().ToList();

                //Apuntadores temporales para guardar el abono almacenado para regenerar el balance
                //despues de guardar el registro de liquidacion
                PrestamoYAbonoCapital abonoAnticipo = new PrestamoYAbonoCapital();
                PrestamoYAbonoCapital abonoArboles = new PrestamoYAbonoCapital();
                //Se editas las retenciones
                foreach (Retencion.TipoRetencion tipo in tiposRetencionesArray)
                {
                    //Retencion previamente registrada
                    var oldRet = retencionesPrevias.FirstOrDefault(mov => mov.tipoDeDeduccion == tipo);

                    //Retencion nueva
                    var newRet = arrRetenciones.FirstOrDefault(mov => mov.tipoDeDeduccion == tipo);
                    
                    //No se habia reportado y se reporta en edicion
                    if (oldRet == null && newRet != null)
                        //Se agrega nueva retencion
                        db.Entry(newRet).State = EntityState.Added;

                    //Existia previamente pero en la edicion se elimina
                    else if (oldRet != null && newRet == null)
                        //Se marca para ser borrada la retencion
                        db.Entry(oldRet).State = EntityState.Deleted;

                    //Se habia reportado previamente y aparece en edicion
                    else if (oldRet != null && newRet != null)
                    {
                        //Se modifica y marca para ser editada
                        oldRet.montoMovimiento = newRet.montoMovimiento;
                        oldRet.fechaMovimiento = newRet.fechaMovimiento;
                        db.Entry(oldRet).State = EntityState.Modified;
                    }
                    //Para retenciones de abono
                    if(tipo == Retencion.TipoRetencion.ABONO_ANTICIPO || tipo == Retencion.TipoRetencion.ABONO_ARBOLES)
                    {
                        PrestamoYAbonoCapital abono = new PrestamoYAbonoCapital();
                        //Se determina el tipo de movimiento capital segun el tipo de rentecion (Abono a anticipo o a arboles)
                        string tipoCapital=string.Empty;
                        decimal montoRetenido = 0;
                        int abonoID = 0;
                        if (tipo == Retencion.TipoRetencion.ABONO_ANTICIPO) { 
                            tipoCapital = PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO;
                            montoRetenido = retenciones.abonoAnticipos;
                            abonoID = emisionDeCheque.abonoAnticipoID == null ? 0 : emisionDeCheque.abonoAnticipoID.Value;
                        }

                        if (tipo == Retencion.TipoRetencion.ABONO_ARBOLES) { 
                            tipoCapital = PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO_ARBOLES;
                            montoRetenido = retenciones.abonoArboles;
                            abonoID = emisionDeCheque.abonoArbolesID == null ? 0 : emisionDeCheque.abonoArbolesID.Value;
                        }

                        //No se habia reportado y se reporta en edicion
                        if (oldRet == null && newRet != null)
                        {
                            //Se crea un nuevo abono como retencion de esta liquidacion
                            abono = PrestamoYAbonoCapital.nuevaRentecionAbono(emisionDeCheque, montoRetenido, tipoCapital);
                            //emisionDeCheque.abonoAnticipo = abono; //Se asocia el nuevo abono

                            if (tipo == Retencion.TipoRetencion.ABONO_ANTICIPO)
                                emisionDeCheque.abonoAnticipo = abono;

                            if (tipo == Retencion.TipoRetencion.ABONO_ARBOLES)
                                emisionDeCheque.abonoArboles = abono;

                            //Se agrega nueva retencion
                            db.Entry(abono).State = EntityState.Added;
                        }
                        //Existia previamente pero en la edicion, se elimina
                        else if (oldRet != null && newRet == null)
                        {
                            abono = db.PrestamosYAbonosCapital.Find(abonoID);
                            
                            //Se desasocia de prestamos que se hayan pagado
                            abono.liberarAbono(db);

                            if (tipo == Retencion.TipoRetencion.ABONO_ANTICIPO)
                                emisionDeCheque.abonoAnticipoID = null;

                            if (tipo == Retencion.TipoRetencion.ABONO_ARBOLES)
                                emisionDeCheque.abonoArbolesID = null;

                            //Se marca para ser borrada la retencion
                            db.Entry(abono).State = EntityState.Deleted;
                        }
                        //Se habia reportado previamente y aparece en edicion
                        else if (oldRet != null && newRet != null)
                        {
                            abono = PrestamoYAbonoCapital.nuevaRentecionAbono(emisionDeCheque, montoRetenido, tipoCapital);
                            abono.idMovimiento = abonoID;
                            db.Entry(abono).State = EntityState.Modified;
                        }

                        if (tipo == Retencion.TipoRetencion.ABONO_ANTICIPO)
                            abonoAnticipo = abono;

                        if (tipo == Retencion.TipoRetencion.ABONO_ARBOLES)
                            abonoArboles = abono;
                    }
                }

                asociarEnEdicion = db.Entry(abonoAnticipo).State == EntityState.Modified;

                db.Entry(emisionDeCheque).State = EntityState.Modified;
                numReg = db.SaveChanges();

                //Si el registro de liquidacion se edito satisfactoriamente
                if (numReg > 0)
                {
                    //Se buscan los registros de ingresos de producto relacioados con la liquidacion
                    var ingresos = db.PagosPorProductos
                        .Where(mov => mov.liquidacionDeCosechaID == emisionDeCheque.idMovimiento).ToList();
                    
                    //Se desasocian
                    ingresos.ForEach(mov =>
                    {
                        mov.liquidacionDeCosechaID = null;
                        db.Entry(mov).State = EntityState.Modified;
                    });
                    db.SaveChanges();
                    ingresos.ForEach(mov => db.Entry(mov).State = EntityState.Detached);

                    //Asociar los nuevos ingresos de cosecha a la liquidacion
                    foreach (int ingID in ingresosDeCosechaID)
                    {
                        var ingreso = db.PagosPorProductos.Find(ingID);
                        ingreso.liquidacionDeCosechaID = emisionDeCheque.idMovimiento;
                        db.Entry(ingreso).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                    //Se valida si se agrego un abono al balance de ANTICIPOS
                    if (abonoAnticipo.idMovimiento > 0)
                    {
                        //Se calcula el movimiento anterior al que se esta registrando
                        var prod = db.Productores.Find(abonoAnticipo.idProductor);
                        MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(abonoAnticipo.fechaMovimiento, abonoAnticipo.tipoDeBalance);
                        
                        //Se asocia el nuevo abono a los prestamos existentes en el balance
                        if (abonoAnticipo.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS)
                            prod.asociarAbonosConPrestamos(db, abonoAnticipo, asociarEnEdicion);

                        //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                        prod.ajustarBalances(ultimoMovimiento, db, abonoAnticipo.tipoDeBalance);
                    }
                    //Se valida si se agrego un abono al balance de ARBOLES
                    if (abonoArboles.idMovimiento > 0)
                    {
                        //Se calcula el movimiento anterior al que se esta registrando
                        var prod = db.Productores.Find(abonoArboles.idProductor);
                        MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(abonoArboles.fechaMovimiento, abonoArboles.tipoDeBalance);

                        //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                        prod.ajustarBalances(ultimoMovimiento, db, abonoArboles.tipoDeBalance);
                    }

                    //numReg = introducirMovimientoAlBalance(emisionDeCheque);
                    return RedirectToAction("ReporteLiquidacionSemanal", new
                        { id = emisionDeCheque.idMovimiento, });
                }
            }

            //En caso de haber problemas en el llenado de la forma, se prepara nuevamente para ser mostrada
            Productor productor = db.Productores.Find(emisionDeCheque.idProductor);
            prepararVistaEditar(ref emisionDeCheque, semanaLiquidada);

            return View("Form_Liquidacion", emisionDeCheque);
        }

        [HttpGet]
        public ActionResult ReporteLiquidacionSemanal(int? id)
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

            prepararVistaEditar(ref mov, null);
            ViewBag.reportMode = true;

            return View("Form_Liquidacion", mov);
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
