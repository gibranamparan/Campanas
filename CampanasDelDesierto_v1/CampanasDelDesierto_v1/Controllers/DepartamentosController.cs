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
    public class DepartamentosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Departamentos
        public async Task<ActionResult> Index()
        {
            var departamentos = db.Departamentos.Include(d => d.Sucursal);
            return View(await departamentos.ToListAsync());
        }

        // GET: Departamentos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamento departamento = await db.Departamentos.FindAsync(id);
            if (departamento == null)
            {
                return HttpNotFound();
            }
            return View(departamento);
        }

        // GET: Departamentos/Create
        public ActionResult Create()
        {
            ViewBag.idSucursal = new SelectList(db.Sucursales, "idSucursal", "nombreSucursal");
            return View();
        }

        // POST: Departamentos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "departamentoID,nombreDepartamento,idSucursal")] Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                db.Departamentos.Add(departamento);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.idSucursal = new SelectList(db.Sucursales, "idSucursal", "nombreSucursal", departamento.idSucursal);
            return View(departamento);
        }

        // GET: Departamentos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamento departamento = await db.Departamentos.FindAsync(id);
            if (departamento == null)
            {
                return HttpNotFound();
            }
            ViewBag.idSucursal = new SelectList(db.Sucursales, "idSucursal", "nombreSucursal", departamento.idSucursal);
            return View(departamento);
        }

        // POST: Departamentos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "departamentoID,nombreDepartamento,idSucursal")] Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departamento).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idSucursal = new SelectList(db.Sucursales, "idSucursal", "nombreSucursal", departamento.idSucursal);
            return View(departamento);
        }

        // GET: Departamentos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamento departamento = await db.Departamentos.FindAsync(id);
            if (departamento == null)
            {
                return HttpNotFound();
            }
            return View(departamento);
        }

        // POST: Departamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Departamento departamento = await db.Departamentos.FindAsync(id);
            db.Departamentos.Remove(departamento);
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
