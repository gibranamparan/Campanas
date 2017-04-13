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
    public class PrestamoYAbonoCapitalsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PrestamoYAbonoCapitals
        public ActionResult Index()
        {
            //var movimientosFinancieros = db.MovimientosFinancieros.Include(p => p.Productor);
            var pretamoyabonocapital = db.PrestamosYAbonosCapital.Include(p => p.Productor);
            return View(pretamoyabonocapital.ToList());
        }

        // GET: PrestamoYAbonoCapitals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoYAbonoCapital prestamoYAbonoCapital = db.PrestamosYAbonosCapital.Find(id);
            if (prestamoYAbonoCapital == null)
            {
                return HttpNotFound();
            }
            return View(prestamoYAbonoCapital);
        }

        // GET: PrestamoYAbonoCapitals/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productor = db.Productores.Find(id);
            if (productor == null)
            {
                return HttpNotFound();
            }

            PrestamoYAbonoCapital mov = prepararVista(productor);

            return View(mov);
        }

        /// <summary>
        /// Se prepara la información que será enviada a la vista de creacion de prestamo
        /// </summary>
        /// <param name="productor"></param>
        /// <returns></returns>
        private PrestamoYAbonoCapital prepararVista(Productor productor)
        {
            ViewBag.productor = productor;
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor");
            ViewBag.idProveedor = new SelectList(db.Proveedores, "id", "nombreProveedor");
            PrestamoYAbonoCapital mov = new PrestamoYAbonoCapital();
            mov.fechaMovimiento = DateTime.Today;
            mov.fechaPagar = DateTime.Today.AddDays(7);
            mov.idProductor = productor.idProductor;

            return mov;
        }

        // POST: PrestamoYAbonoCapitals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,cheque,concepto,pagare,fechaPagar,proveedor,nota")] PrestamoYAbonoCapital prestamoYAbonoCapital)
        {
            if (ModelState.IsValid)
            {
                //Se calcula el ultimo movimiento antes de guardar el nuevo
                var prod = db.Productores.Find(prestamoYAbonoCapital.idProductor);
                MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(prestamoYAbonoCapital.fechaMovimiento);

                db.PrestamosYAbonosCapital.Add(prestamoYAbonoCapital);
                int numReg = db.SaveChanges();
                if (numReg > 0)
                {
                    prod.ajustarBalances(ultimoMovimiento);
                }
                return RedirectToAction("Details","Productores",new { id = prestamoYAbonoCapital.idProductor});
            }

            prepararVista(db.Productores.Find(prestamoYAbonoCapital.idProductor));

            return View(prestamoYAbonoCapital);
        }

        // GET: PrestamoYAbonoCapitals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoYAbonoCapital prestamoYAbonoCapital = db.PrestamosYAbonosCapital.Find(id);
            if (prestamoYAbonoCapital == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", prestamoYAbonoCapital.idProductor);
            return View(prestamoYAbonoCapital);
        }

        // POST: PrestamoYAbonoCapitals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,fechaDePrestamo,cheque,concepto,cargo,pagare,fechaPagar,proveedor,nota")] PrestamoYAbonoCapital prestamoYAbonoCapital)
        {
            if (ModelState.IsValid)
            {
                db.Entry(prestamoYAbonoCapital).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", prestamoYAbonoCapital.idProductor);
            return View(prestamoYAbonoCapital);
        }

        // GET: PrestamoYAbonoCapitals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoYAbonoCapital prestamoYAbonoCapital = db.PrestamosYAbonosCapital.Find(id);
            if (prestamoYAbonoCapital == null)
            {
                return HttpNotFound();
            }
            return View(prestamoYAbonoCapital);
        }

        // POST: PrestamoYAbonoCapitals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PrestamoYAbonoCapital prestamoYAbonoCapital = db.PrestamosYAbonosCapital.Find(id);
            db.MovimientosFinancieros.Remove(prestamoYAbonoCapital);
            db.SaveChanges();
            return RedirectToAction("Index");
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
