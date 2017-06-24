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
            return View(movimientoFinanciero);
        }

        // GET: MovimientoFinancieros/Details/5
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public ActionResult Pagare(int? id)
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
            return View(movimientoFinanciero);
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
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor")] MovimientoFinanciero movimientoFinanciero)
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
            int temporadaID = mov.TemporadaDeCosechaID;
            var prod = db.Productores.Find(mov.idProductor);
            bool retencionAbonoEliminado = false;

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
                if (((LiquidacionSemanal)mov).abonoAnticipo != null) {
                    db.PrestamosYAbonosCapital.Remove(((LiquidacionSemanal)mov).abonoAnticipo);
                    retencionAbonoEliminado = true;
                }
            }

            //se elimina el movimiento
            db.MovimientosFinancieros.Remove(mov);
            int numReg = db.SaveChanges();

            //Se ajusta el balance si si es un movimiento financierto que no sea registro de cosecha (pago por producto),
            //se ajusta el balance si se registra una liquidacion en cuyas renteciones se encuentre un abono a deudas
            if (numReg > 0 && prod.MovimientosFinancieros.Count() > 0
                && mov.getTypeOfMovement() != MovimientoFinanciero.TypeOfMovements.PAGO_POR_PRODUCTO
                && (mov.getTypeOfMovement() != MovimientoFinanciero.TypeOfMovements.LIQUIDACION || retencionAbonoEliminado))
            {
                //Se calcula el ultimo movimiento anterior al que se desea eliminar
                MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(mov.fechaMovimiento);

                //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                numReg = prod.ajustarBalances(ultimoMovimiento, db);
            }
            
            return RedirectToAction("Details", "Productores", new { id = mov.idProductor, temporada= temporadaID });
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
