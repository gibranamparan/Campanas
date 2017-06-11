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

        // public ActionResult GeneratePDF()
        //{
        //  if (User.IsInRole(ApplicationUser.RoleNames.ADMIN))
        //{
        //  return new Rotativa.ActionAsPdf("Pagare");
        //}
        //else
        //{
        //  return RedirectToAction("Index");
        //}
        //}




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

                //Y el abono a deudas o anticipos
                if (((LiquidacionSemanal)mov).abonoAnticipo != null) {
                    db.PrestamosYAbonosCapital.Remove(((LiquidacionSemanal)mov).abonoAnticipo);
                    retencionAbonoEliminado = true;
                }

            }

            //se elimina el movimiento
            db.MovimientosFinancieros.Remove(mov);
            int numReg = db.SaveChanges();

            if (numReg > 0 && prod.MovimientosFinancieros.Count() > 0
                && mov.getTypeOfMovement() != MovimientoFinanciero.TypeOfMovements.PAGO_POR_PRODUCTO
                && (mov.getTypeOfMovement() != MovimientoFinanciero.TypeOfMovements.LIQUIDACION || retencionAbonoEliminado))
            {
                //Se calcula el ultimo movimiento anterior al que se desea eliminar
                MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(mov.fechaMovimiento);

                //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                numReg = prod.ajustarBalances(ultimoMovimiento, db);
            }
            
            return RedirectToAction("Details", "Productores", new { id = mov.idProductor });
            //return RedirectToAction("Index");
        }

        [HttpPost, ValidateHeaderAntiForgeryToken]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
        public JsonResult getCambioDolar()
        {
            String errorMsg = "";
            BaxicoWebService bws = new BaxicoWebService();
            decimal precioDolar = bws.getCambioDolar(ref errorMsg);
            return Json(new { precioDolar = precioDolar, errorMsg = errorMsg });
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
