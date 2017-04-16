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
    [Authorize(Roles ="Admin")]
    public class ProductoresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Productores
        public ActionResult Index()
        {
            return View(db.Productores.ToList());
        }

        // GET: Productores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Productor productor = db.Productores.Find(id);
            Productor productor = db.Productores.Include("MovimientosFinancieros").Single(pro=>pro.idProductor == id);
            if (productor == null)
            {
                return HttpNotFound();
            }
            return View(productor);
        }

        // GET: Productores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Productores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idProductor,nombreProductor,domicilio,fechaIntegracion,RFC,zona,nombreCheque,adeudoAnterior,precioCosecha")] Productor productor)
        {
            if (ModelState.IsValid)
            {
                db.Productores.Add(productor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productor);
        }

        // GET: Productores/Edit/5
        public ActionResult Edit(int? id)
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
            return View(productor);
        }

        // POST: Productores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idProductor,nombreProductor,domicilio,fechaIntegracion,RFC,zona,nombreCheque,adeudoAnterior,precioCosecha")] Productor productor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productor);
        }

        // GET: Productores/Delete/5
        public ActionResult Delete(int? id)
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
            return View(productor);
        }

        // POST: Productores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Productor productor = db.Productores.Find(id);
            db.Productores.Remove(productor);
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
