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
    public class PrestamosMaterialesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PrestamosMateriales
        public ActionResult Index()
        {
            var movimientosFinancieros = db.PrestamoMateriales.Include(p => p.Productor).Include(p => p.Activo);
            return View(movimientosFinancieros.ToList());
        }

        // GET: PrestamosMateriales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoMaterial prestamoMaterial = db.PrestamoMateriales.Find(id);
            if (prestamoMaterial == null)
            {
                return HttpNotFound();
            }
            return View(prestamoMaterial);
        }

        // GET: PrestamosMateriales/Create
        public ActionResult Create()
        {
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor");
            ViewBag.idActivos = new SelectList(db.Activos, "idActivos", "nombreActivo");
            return View();
        }

        // POST: PrestamosMateriales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,cantidadMaterial,montoMovimiento,fechaMovimiento,idProductor,idActivos")] PrestamoMaterial prestamoMaterial)
        {
            for (int i = 1; i <= prestamoMaterial.cantidadMaterial; i++)
            {
                var idActivos = prestamoMaterial.idActivos;
                var activo = db.Activos.Find(idActivos);
                prestamoMaterial.montoMovimiento = (int)activo.costo;

                if (ModelState.IsValid)
                {
                    db.MovimientosFinancieros.Add(prestamoMaterial);
                    db.SaveChanges();
                    
                }
                
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", prestamoMaterial.idProductor);
            ViewBag.idActivos = new SelectList(db.Activos, "idActivos", "nombreActivo", prestamoMaterial.idActivos);            
            return  RedirectToAction("Index");

        }

        // GET: PrestamosMateriales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoMaterial prestamoMaterial = db.PrestamoMateriales.Find(id);
            if (prestamoMaterial == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", prestamoMaterial.idProductor);
            ViewBag.idActivos = new SelectList(db.Activos, "idActivos", "nombreActivo", prestamoMaterial.idActivos);
            return View(prestamoMaterial);
        }

        // POST: PrestamosMateriales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,idActivos")] PrestamoMaterial prestamoMaterial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(prestamoMaterial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", prestamoMaterial.idProductor);
            ViewBag.idActivos = new SelectList(db.Activos, "idActivos", "nombreActivo", prestamoMaterial.idActivos);
            return View(prestamoMaterial);
        }

        // GET: PrestamosMateriales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoMaterial prestamoMaterial = db.PrestamoMateriales.Find(id);
            if (prestamoMaterial == null)
            {
                return HttpNotFound();
            }
            return View(prestamoMaterial);
        }

        // POST: PrestamosMateriales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PrestamoMaterial prestamoMaterial = db.PrestamoMateriales.Find(id);
            db.MovimientosFinancieros.Remove(prestamoMaterial);
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
