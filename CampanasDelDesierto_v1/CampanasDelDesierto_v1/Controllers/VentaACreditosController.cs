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
    public class VentaACreditosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: VentaACreditos
        public ActionResult Index()
        {
            //var movimientosFinancieros = db.MovimientosFinancieros.Include(v => v.Productor);
            var ventaacredito = db.VentasACreditos.Include(v => v.idProductor);
            return View(ventaacredito.ToList());
        }

        // GET: VentaACreditos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VentaACredito ventaACredito = db.VentasACreditos.Find(id);
            if (ventaACredito == null)
            {
                return HttpNotFound();
            }
            return View(ventaACredito);
        }

        // GET: VentaACreditos/Create
        public ActionResult Create()
        {
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor");
            ViewBag.idActivos = new SelectList(db.Activos, "idActivos", "nombreActivo");
            return View();
        }

        // POST: VentaACreditos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,cantidadMaterial,idActivos")] VentaACredito ventaACredito)
        {
            if (ModelState.IsValid)
            {
                db.MovimientosFinancieros.Add(ventaACredito);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", ventaACredito.idProductor);
            ViewBag.idActivos = new SelectList(db.Activos, "idActivos", "nombreActivo", ventaACredito.idActivos);
            return View(ventaACredito);
        }

        // GET: VentaACreditos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VentaACredito ventaACredito = db.VentasACreditos.Find(id);
            if (ventaACredito == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", ventaACredito.idProductor);
            ViewBag.idActivos = new SelectList(db.Activos, "idActivos", "nombreActivo", ventaACredito.idActivos);
            return View(ventaACredito);
        }

        // POST: VentaACreditos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,cantidadMaterial,idActivos")] VentaACredito ventaACredito)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ventaACredito).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", ventaACredito.idProductor);
            ViewBag.idActivos = new SelectList(db.Activos, "idActivos", "nombreActivo", ventaACredito.idActivos);
            return View(ventaACredito);
        }

        // GET: VentaACreditos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VentaACredito ventaACredito = db.VentasACreditos.Find(id);
            if (ventaACredito == null)
            {
                return HttpNotFound();
            }
            return View(ventaACredito);
        }

        // POST: VentaACreditos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VentaACredito ventaACredito = db.VentasACreditos.Find(id);
            db.MovimientosFinancieros.Remove(ventaACredito);
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
