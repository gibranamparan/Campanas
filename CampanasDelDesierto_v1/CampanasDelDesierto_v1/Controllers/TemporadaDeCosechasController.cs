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
    public class TemporadaDeCosechasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TemporadaDeCosechas
        public ActionResult Index()
        {
            return View(db.TemporadaDeCosechas.ToList());
        }

        // GET: TemporadaDeCosechas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TemporadaDeCosecha temporadaDeCosecha = db.TemporadaDeCosechas.Find(id);
            if (temporadaDeCosecha == null)
            {
                return HttpNotFound();
            }
            return View(temporadaDeCosecha);
        }

        // GET: TemporadaDeCosechas/Create
        public ActionResult Create()
        {
            TemporadaDeCosecha tem = new TemporadaDeCosecha();
            return View(tem);
        }

        // POST: TemporadaDeCosechas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TemporadaDeCosechaID,fechaInicio,fechaFin,precioProducto1,"+
            "precioProducto2,precioProducto3,precioProductoOtro")]
            TemporadaDeCosecha temporadaDeCosecha)
        {
            if (ModelState.IsValid)
            {
                db.TemporadaDeCosechas.Add(temporadaDeCosecha);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(temporadaDeCosecha);
        }

        // GET: TemporadaDeCosechas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TemporadaDeCosecha temporadaDeCosecha = db.TemporadaDeCosechas.Find(id);
            if (temporadaDeCosecha == null)
            {
                return HttpNotFound();
            }
            return View(temporadaDeCosecha);
        }

        // POST: TemporadaDeCosechas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TemporadaDeCosechaID,fechaInicio,fechaFin,"+
            "precioProducto1,precioProducto2,precioProducto3,precioProductoOtro")]
            TemporadaDeCosecha temporadaDeCosecha)
        {
            if (ModelState.IsValid)
            {
                db.Entry(temporadaDeCosecha).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(temporadaDeCosecha);
        }

        // GET: TemporadaDeCosechas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TemporadaDeCosecha temporadaDeCosecha = db.TemporadaDeCosechas.Find(id);
            if (temporadaDeCosecha == null)
            {
                return HttpNotFound();
            }
            return View(temporadaDeCosecha);
        }

        // POST: TemporadaDeCosechas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TemporadaDeCosecha temporadaDeCosecha = db.TemporadaDeCosechas.Find(id);
            db.TemporadaDeCosechas.Remove(temporadaDeCosecha);
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
