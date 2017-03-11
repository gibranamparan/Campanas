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
    public class PrestamosCapitalesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PrestamosCapitales
        public ActionResult Index()
        {
            var movimientosFinancieros = db.PrestamoCapital.Include(p => p.Productor);
            return View(movimientosFinancieros.ToList());
        }

        // GET: PrestamosCapitales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoCapital prestamoCapital = db.PrestamoCapital.Find(id);
            if (prestamoCapital == null)
            {
                return HttpNotFound();
            }
            return View(prestamoCapital);
        }

        // GET: PrestamosCapitales/Create
        public ActionResult Create()
        {
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor");
            return View();
        }

        // POST: PrestamosCapitales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,MesesAPagar")] PrestamoCapital prestamoCapital)
        {
            if (ModelState.IsValid)
            {
                db.MovimientosFinancieros.Add(prestamoCapital);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", prestamoCapital.idProductor);
            return View(prestamoCapital);
        }

        // GET: PrestamosCapitales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoCapital prestamoCapital = db.PrestamoCapital.Find(id);
            if (prestamoCapital == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", prestamoCapital.idProductor);
            return View(prestamoCapital);
        }

        // POST: PrestamosCapitales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,MesesAPagar")] PrestamoCapital prestamoCapital)
        {
            if (ModelState.IsValid)
            {
                db.Entry(prestamoCapital).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", prestamoCapital.idProductor);
            return View(prestamoCapital);
        }

        // GET: PrestamosCapitales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoCapital prestamoCapital = db.PrestamoCapital.Find(id);
            if (prestamoCapital == null)
            {
                return HttpNotFound();
            }
            return View(prestamoCapital);
        }

        // POST: PrestamosCapitales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PrestamoCapital prestamoCapital = db.PrestamoCapital.Find(id);
            db.MovimientosFinancieros.Remove(prestamoCapital);
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
