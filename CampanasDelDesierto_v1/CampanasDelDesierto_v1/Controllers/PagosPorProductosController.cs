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
    public class PagosPorProductosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PagosPorProductos
        public ActionResult Index()
        {
            var movimientosFinancieros = db.PagoPorProductos.Include(p => p.Productor);
            return View(movimientosFinancieros.ToList());
        }

        // GET: PagosPorProductos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PagoPorProducto pagoPorProducto = db.PagoPorProductos.Find(id);
            if (pagoPorProducto == null)
            {
                return HttpNotFound();
            }
            return View(pagoPorProducto);
        }

        // GET: PagosPorProductos/Create
        public ActionResult Create()
        {
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor");
            return View();
        }

        // POST: PagosPorProductos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,cantidadProducto")] PagoPorProducto pagoPorProducto)
        {
            if (ModelState.IsValid)
            {
                db.MovimientosFinancieros.Add(pagoPorProducto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", pagoPorProducto.idProductor);
            return View(pagoPorProducto);
        }

        // GET: PagosPorProductos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PagoPorProducto pagoPorProducto = db.PagoPorProductos.Find(id);
            if (pagoPorProducto == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", pagoPorProducto.idProductor);
            return View(pagoPorProducto);
        }

        // POST: PagosPorProductos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,cantidadProducto")] PagoPorProducto pagoPorProducto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pagoPorProducto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", pagoPorProducto.idProductor);
            return View(pagoPorProducto);
        }

        // GET: PagosPorProductos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PagoPorProducto pagoPorProducto = db.PagoPorProductos.Find(id);
            if (pagoPorProducto == null)
            {
                return HttpNotFound();
            }
            return View(pagoPorProducto);
        }

        // POST: PagosPorProductos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PagoPorProducto pagoPorProducto = db.PagoPorProductos.Find(id);
            db.MovimientosFinancieros.Remove(pagoPorProducto);
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
