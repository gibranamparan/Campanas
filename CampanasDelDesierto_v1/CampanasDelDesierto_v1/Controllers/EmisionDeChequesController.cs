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
    public class EmisionDeChequesController : Controller
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
            EmisionDeCheque emisionDeCheque = db.EmisionDeCheques.Find(id);
            if (emisionDeCheque == null)
            {
                return HttpNotFound();
            }
            return View(emisionDeCheque);
        }

        // GET: EmisionDeCheques/Create
        public ActionResult Create()
        {
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor");
            ViewBag.TemporadaDeCosechaID = new SelectList(db.TemporadaDeCosechas, "TemporadaDeCosechaID", "TemporadaDeCosechaID");
            return View();
        }

        // POST: EmisionDeCheques/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,balance,fechaMovimiento,idProductor,TemporadaDeCosechaID,cheque,abonoAnticipo,garantiaLimpieza")] EmisionDeCheque emisionDeCheque)
        {
            if (ModelState.IsValid)
            {
                db.MovimientosFinancieros.Add(emisionDeCheque);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", emisionDeCheque.idProductor);
            ViewBag.TemporadaDeCosechaID = new SelectList(db.TemporadaDeCosechas, "TemporadaDeCosechaID", "TemporadaDeCosechaID", emisionDeCheque.TemporadaDeCosechaID);
            return View(emisionDeCheque);
        }

        // GET: EmisionDeCheques/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmisionDeCheque emisionDeCheque = db.EmisionDeCheques.Find(id);
            if (emisionDeCheque == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", emisionDeCheque.idProductor);
            ViewBag.TemporadaDeCosechaID = new SelectList(db.TemporadaDeCosechas, "TemporadaDeCosechaID", "TemporadaDeCosechaID", emisionDeCheque.TemporadaDeCosechaID);
            return View(emisionDeCheque);
        }

        // POST: EmisionDeCheques/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,balance,fechaMovimiento,idProductor,TemporadaDeCosechaID,cheque,abonoAnticipo,garantiaLimpieza")] EmisionDeCheque emisionDeCheque)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emisionDeCheque).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", emisionDeCheque.idProductor);
            ViewBag.TemporadaDeCosechaID = new SelectList(db.TemporadaDeCosechas, "TemporadaDeCosechaID", "TemporadaDeCosechaID", emisionDeCheque.TemporadaDeCosechaID);
            return View(emisionDeCheque);
        }

        // GET: EmisionDeCheques/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmisionDeCheque emisionDeCheque = db.EmisionDeCheques.Find(id);
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
            EmisionDeCheque emisionDeCheque = db.EmisionDeCheques.Find(id);
            db.MovimientosFinancieros.Remove(emisionDeCheque);
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
