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
    [Authorize(Roles = "Admin, Sucursal")]
    public class ActivosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Activos
        public ActionResult Index()
        {
            return View(db.Activos.ToList());
        }

        // GET: Activos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activo activo = db.Activos.Find(id);
            if (activo == null)
            {
                return HttpNotFound();
            }
            return View(activo);
        }

        // GET: Activos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Activos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idActivo,nombreActivo,estadoActivo")] Activo activo)
        {
            if (ModelState.IsValid)
            {
                db.Activos.Add(activo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(activo);
        }

        // GET: Activos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activo activo = db.Activos.Find(id);
            if (activo == null)
            {
                return HttpNotFound();
            }
            return View(activo);
        }

        // POST: Activos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idActivo,nombreActivo,estadoActivo")] Activo activo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activo);
        }

        // GET: Activos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activo activo = db.Activos.Find(id);
            if (activo == null)
            {
                return HttpNotFound();
            }
            return View(activo);
        }

        // POST: Activos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Activo activo = db.Activos.Find(id);
            db.Activos.Remove(activo);
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
