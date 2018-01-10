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
    public class ProductosActivosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductosActivos
        public ActionResult Index()
        {
            var productosActivos = db.ProductosActivos.Include(p => p.Activo);
            return View(productosActivos.ToList());
        }

        // GET: ProductosActivos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductoActivo productoActivo = db.ProductosActivos.Find(id);
            if (productoActivo == null)
            {
                return HttpNotFound();
            }
            return View(productoActivo);
        }

        // GET: ProductosActivos/Create
        public ActionResult Create()
        {
            ViewBag.idActivo = new SelectList(db.Activos, "idActivo", "partidaNumActivo");
            return View();
        }

        // POST: ProductosActivos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductoActivoID,noSerie,descripcionActivo,observacionesActivo,fechaPrestamo,fechaDevolucion,fechaEntregado,idActivo")] ProductoActivo productoActivo)
        {
            if (ModelState.IsValid)
            {
                db.ProductosActivos.Add(productoActivo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idActivo = new SelectList(db.Activos, "idActivo", "partidaNumActivo", productoActivo.idActivo);
            return View(productoActivo);
        }

        // GET: ProductosActivos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductoActivo productoActivo = db.ProductosActivos.Find(id);
            if (productoActivo == null)
            {
                return HttpNotFound();
            }
            ViewBag.idActivo = new SelectList(db.Activos, "idActivo", "partidaNumActivo", productoActivo.idActivo);
            return View(productoActivo);
        }

        // POST: ProductosActivos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductoActivoID,noSerie,descripcionActivo,observacionesActivo,fechaPrestamo,fechaDevolucion,fechaEntregado,idActivo")] ProductoActivo productoActivo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productoActivo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idActivo = new SelectList(db.Activos, "idActivo", "partidaNumActivo", productoActivo.idActivo);
            return View(productoActivo);
        }

        // GET: ProductosActivos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductoActivo productoActivo = db.ProductosActivos.Find(id);
            if (productoActivo == null)
            {
                return HttpNotFound();
            }
            return View(productoActivo);
        }

        // POST: ProductosActivos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductoActivo productoActivo = db.ProductosActivos.Find(id);
            db.ProductosActivos.Remove(productoActivo);
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
