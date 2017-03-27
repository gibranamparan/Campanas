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
    public class MovimientoFinancierosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MovimientoFinancieros
        public ActionResult Index()
        {
            var movimientosFinancieros = db.MovimientosFinancieros.Include(m => m.Productor);
            return View(movimientosFinancieros.ToList());
        }

        // GET: MovimientoFinancieros/Details/5
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

        // GET: MovimientoFinancieros/Create
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
        public ActionResult DeleteConfirmed(int id)
        {
            MovimientoFinanciero movimientoFinanciero = db.MovimientosFinancieros.Find(id);
            db.MovimientosFinancieros.Remove(movimientoFinanciero);
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
