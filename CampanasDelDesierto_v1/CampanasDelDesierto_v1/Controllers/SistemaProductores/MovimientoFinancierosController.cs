using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;
using static CampanasDelDesierto_v1.HerramientasGenerales.FiltrosDeSolicitudes;
using CampanasDelDesierto_v1.HerramientasGenerales;
using System.IO;
using OfficeOpenXml;
using System.ComponentModel;
using static CampanasDelDesierto_v1.Models.PrestamoYAbonoCapital;

namespace CampanasDelDesierto_v1.Controllers
{

    public class MovimientoFinancierosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MovimientoFinancieros
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Index()
        {
            var movimientosFinancieros = db.MovimientosFinancieros.Include(m => m.Productor);
            return View(movimientosFinancieros.ToList());
        }

        // GET: MovimientoFinancieros/Details/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientoFinanciero movimientoFinanciero = db.MovimientosFinancieros.Find(id);
            if (movimientoFinanciero == null)
            {
                return HttpNotFound();
            }

            if(movimientoFinanciero.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.CAPITAL)
            {
                var mov = db.PrestamosYAbonosCapital.Find(id);
                var abonos = mov.abonosRecibidos.ToList();
                var prestamos = mov.prestamosAbonados.ToList();
            }

            return View(movimientoFinanciero);
        }

        // GET: MovimientoFinancieros/Details/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult PagareAnticipo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoYAbonoCapital movimientoFinanciero = db.PrestamosYAbonosCapital.Find(id);
            if (movimientoFinanciero == null)
            {
                return HttpNotFound();
            }
            return View("Pagare", movimientoFinanciero);
        }
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]        
        public ActionResult PagareVenta(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VentaACredito movimientoFinanciero = db.VentasACreditos.Find(id);
            if (movimientoFinanciero == null)
            {
                return HttpNotFound();
            }
            return View("Pagare", movimientoFinanciero);
        }

        // GET: MovimientoFinancieros/Create
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Create()
        {
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor");
            return View();
        }

        // POST: MovimientoFinancieros/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor")] MovimientoFinanciero movimientoFinanciero)
        {
            if (ModelState.IsValid)
            {

                db.MovimientosFinancieros.Add(movimientoFinanciero);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", movimientoFinanciero.idProductor);
            return View(movimientoFinanciero);
        }
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        // GET: MovimientoFinancieros/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientoFinanciero movimientoFinanciero = db.MovimientosFinancieros.Find(id);
            if (movimientoFinanciero == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", movimientoFinanciero.idProductor);
            return View(movimientoFinanciero);
        }

        // POST: MovimientoFinancieros/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor")]
            MovimientoFinanciero movimientoFinanciero)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movimientoFinanciero).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", movimientoFinanciero.idProductor);
            return View(movimientoFinanciero);
        }



        // GET: MovimientoFinancieros/Delete/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientoFinanciero movimientoFinanciero = db.MovimientosFinancieros.Find(id);
            if (movimientoFinanciero == null)
            {
                return HttpNotFound();
            }
            return View(movimientoFinanciero);
        }

        // POST: MovimientoFinancieros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult DeleteConfirmed(int id)
        {
            MovimientoFinanciero mov = db.MovimientosFinancieros.Find(id);
            int temporadaID = mov.TemporadaDeCosechaID, numReg = 0;
            var prod = db.Productores.Find(mov.idProductor);
            MovimientoFinanciero abonoAnticiposEliminado=null, abonoArbolesEliminado = null;

            if(!mov.isNoDirectamenteModificable) { 
                //Los registros de recibos se desasocian de los registro de PagoPorProducto para estar disponibles
                //nuevamente para otros registros.
                if (mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.PAGO_POR_PRODUCTO)
                    ((PagoPorProducto)mov).eliminarAsociacionConRecepciones(db);

                //En caso de se la eliminacion de un movimiento de liquidacion
                if (mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.LIQUIDACION)
                {
                    //Se eliminan sus correspondientes retenciones
                    if (((LiquidacionSemanal)mov).retenciones != null && ((LiquidacionSemanal)mov).retenciones.Count() > 0)
                        db.Retenciones.RemoveRange(((LiquidacionSemanal)mov).retenciones);

                    //Se libera la sociacion de los ingresos de cosecha de la liquidacion que se esta eliminando
                    if (((LiquidacionSemanal)mov).ingresosDeCosecha != null && ((LiquidacionSemanal)mov).ingresosDeCosecha.Count() > 0)
                        ((LiquidacionSemanal)mov).ingresosDeCosecha.ToList()
                            .ForEach(ing => {
                                ing.liquidacionDeCosechaID = null;
                                db.Entry(ing).State = EntityState.Modified;
                            });

                    //Se elimina el abono a deudas o anticipos registrado como retencion en la liquidacion
                    if (((LiquidacionSemanal)mov).abonoAnticipo != null)

                    {
                        //Se guarda la instancia del abono eliminado para uso en ajustes de balances y distribuciones
                        abonoAnticiposEliminado = ((LiquidacionSemanal)mov).abonoAnticipo;
                        //Se libera el abono de los posibles pagos que se hayan tomado de el
                        numReg = ((LiquidacionSemanal)mov).abonoAnticipo.liberarAbono(db);
                        //Se remueve el abono de la base de datos
                        db.PrestamosYAbonosCapital.Remove(((LiquidacionSemanal)mov).abonoAnticipo);
                        //retencionAbonoEliminado = true;
                    }
                    //Se elimina el abono a deudas o anticipos registrado como retencion en la liquidacion
                    if (((LiquidacionSemanal)mov).abonoArboles != null)
                    {
                        //Se guarda la instancia del abono eliminado para uso en ajustes de balances
                        abonoArbolesEliminado = ((LiquidacionSemanal)mov).abonoArboles;
                        //Se remueve el abono de la base de datos
                        db.PrestamosYAbonosCapital.Remove(((LiquidacionSemanal)mov).abonoArboles);
                        //retencionAbonoArbolesEliminado = true;
                    }
                }

                //Si es esta eliminando un abono del balance de anticipos, 
                //se elimina su asociacion de pagos a anticipos
                if(mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS) {
                    if (mov.isAbonoCapital)
                        numReg = ((PrestamoYAbonoCapital)mov).liberarAbono(db);
                    else
                        numReg = mov.liberarPrestamo(db);
                }

                //se elimina el movimiento
                db.MovimientosFinancieros.Remove(mov);
                numReg = db.SaveChanges();

                //Si la eliminacion del movimiento se realizo satisfactoriamente
                if (numReg > 0 && prod.MovimientosFinancieros.Count() > 0)
                {
                    List<Prestamo_Abono> nuevasAsoc = new List<Prestamo_Abono>();
                    MovimientoFinanciero ultimoMovimiento;
                
                    //Si se elimina un movimiento del balance de anticipos y materiales o bien
                    //al eliminar una liquidacion, se elimino una retencion de abono para dicho balance
                    if (mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS 
                        || (mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.MOV_LIQUIDACION && abonoAnticiposEliminado != null))
                    {
                        nuevasAsoc = prod.asociarAbonosConPrestamos(db, abonoAnticiposEliminado); //Se redistribuyen abonos
                        ultimoMovimiento = encontrarPrimerMovimientoAfectado(nuevasAsoc); //Se determina el movimiento afectado mas viejo en la redistribucion
                        ultimoMovimiento = ultimoMovimiento == null ? mov : ultimoMovimiento;
                        //A partir del movimiento anterior las mas viejo, se recalculan balances
                        ultimoMovimiento = prod.getUltimoMovimiento(ultimoMovimiento.fechaMovimiento, MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS);
                        numReg = prod.ajustarBalances(ultimoMovimiento, db, MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS);
                    }

                    //Si se elimina un movimiento del balance de anticipos y materiales o bien
                    //al eliminar una liquidacion, se elimino una retencion de abono para dicho balance
                    if (mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO
                        || (mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.MOV_LIQUIDACION && abonoArbolesEliminado != null))
                    {
                        //A partir del movimiento anterior al abono a arboles eliminado, se recalculan balances
                        ultimoMovimiento = prod.getUltimoMovimiento(abonoArbolesEliminado.fechaMovimiento, MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO);
                        numReg = prod.ajustarBalances(ultimoMovimiento, db, MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO);
                    }
                }
            }

            return RedirectToAction("Details", "Productores", new { id = mov.idProductor, temporada= temporadaID });
        }

        private MovimientoFinanciero encontrarPrimerMovimientoAfectado(List<Prestamo_Abono> nuevasAsoc)
        {
            MovimientoFinanciero res;
            //se busca el movimiento mas viejo afectado la redistribucion de prestamos y abonos.
            var abonos = nuevasAsoc.Where(m => m.abono != null).OrderBy(m => m.abono.fechaMovimiento);
            var prestamos = nuevasAsoc.Where(m => m.prestamo != null).OrderBy(m => m.prestamo.fechaMovimiento);

            var abono = abonos != null && abonos.Count() > 0 ? abonos.FirstOrDefault() : null; //Abono mas viejo
            var prestamo = prestamos != null && prestamos.Count() > 0 ? prestamos.FirstOrDefault() : null; //Prestamo mas viejo

            if (abono == null && prestamo == null)
                res = null; //SI NO
            else if (abono == null) //Si no existe uno, es el otro
                res = prestamo.prestamo;
            else if (prestamo == null)
                res = abono.abono;
            else //Si existen ambos, se toma el mas viejo
                res = abono.abono.fechaMovimiento <= prestamo.prestamo.fechaMovimiento ? abono.abono : prestamo.prestamo;

            return res;
        }

        /// <summary>
        /// Obtiene el tipo de cambio segun el día de la fecha de consulta segun 
        /// el webservice de Banxico
        /// </summary>
        /// <returns></returns>
        [HttpPost, ValidateHeaderAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public JsonResult getCambioDolar()
        {
            String errorMsg = "";
            BaxicoWebService bws = new BaxicoWebService();
            decimal precioDolar = bws.getCambioDolar(ref errorMsg);
            return Json(new { precioDolar = precioDolar, errorMsg = errorMsg });
        }

        public JsonResult getFileFromSAT(int year)
        {
            BaxicoWebService bws = new BaxicoWebService();
            string errorMsg = string.Empty;
            bool res = bws.getExcelDocFromSat(year, ref errorMsg);

            return Json(new { success = res, errorMsg = errorMsg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult saveDollarRateFromExcel(int year)
        {
            string pathDestino = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            string url = $"{pathDestino}\\tc_{year}.xls";

            //Se crea el archivo Excel procesable
            var package = new ExcelPackage(new FileInfo(@url));
            //var workSheet = currentSheet.First();//Se toma la 1ra hoja de excel
            var workSheet = package.Workbook.Worksheets[1];//Se toma la 1ra hoja de excel
            var noOfCol = workSheet.Dimension.End.Column;//Se determina el ancho de la tabla en no. columnas
            var noOfRow = workSheet.Dimension.End.Row;//El alto de la tabla en numero de renglores

            return Json("dsda", JsonRequestBehavior.AllowGet);
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
