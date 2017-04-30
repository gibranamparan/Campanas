using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;

namespace CampanasDelDesierto_v1.Controllers
{
    [Authorize]
    public class ActivosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Activos
        public async Task<ActionResult> Index()
        {
            var activos = db.Activos.Include(a => a.inventario);
            return View(await activos.ToListAsync());
        }

        // GET: Activos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activo activo = await db.Activos.FindAsync(id);
            if (activo == null)
            {
                return HttpNotFound();
            }
            return View(activo);
        }

        // GET: Activos/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                ViewBag.inventarioID = new SelectList(db.Inventarios, "inventarioID", "nombreInventario");
            }
            else
            {
                Inventario Inventario = db.Inventarios.Find(id);
                ViewBag.inventarioID = new SelectList(db.Inventarios, "inventarioID", "nombreInventario",Inventario.inventarioID);
            }
                

            return View();
        }

        // POST: Activos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idActivo,nombreActivo,estadoActivo,inventarioID")] Activo activo)
        {
            if (ModelState.IsValid)
            {
                db.Activos.Add(activo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.inventarioID = new SelectList(db.Inventarios, "inventarioID", "nombreInventario", activo.inventarioID);
            return View(activo);
        }

        // GET: Activos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activo activo = await db.Activos.FindAsync(id);
            if (activo == null)
            {
                return HttpNotFound();
            }
            ViewBag.inventarioID = new SelectList(db.Inventarios, "inventarioID", "nombreInventario", activo.inventarioID);
            return View(activo);
        }

        // POST: Activos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idActivo,nombreActivo,estadoActivo,inventarioID")] Activo activo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.inventarioID = new SelectList(db.Inventarios, "inventarioID", "nombreInventario", activo.inventarioID);
            return View(activo);
        }

        // GET: Activos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activo activo = await db.Activos.FindAsync(id);
            if (activo == null)
            {
                return HttpNotFound();
            }
            return View(activo);
        }

        // POST: Activos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Activo activo = await db.Activos.FindAsync(id);
            db.Activos.Remove(activo);
            await db.SaveChangesAsync();
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
