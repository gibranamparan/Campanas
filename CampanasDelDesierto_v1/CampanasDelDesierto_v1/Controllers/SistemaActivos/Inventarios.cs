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
    public class InventariosController : Controller
    {
        //Bind de categoria
        private const string bindCategoriaActivo = "CategoriaActivoID,nombreCategoria";
        //Conexion a la base de datos
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Inventarios
        public async Task<ActionResult> Index()
        {
            var CategoriasDeActivos = db.CategoriasDeActivos;           
            return View(await CategoriasDeActivos.ToListAsync());
        }

        // GET: Inventarios/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaDeActivo inventario = await db.CategoriasDeActivos.FindAsync(id);
            if (inventario == null)
            {
                return HttpNotFound();
            }
            return View(inventario);
        }

        // GET: Inventarios/Create
        public ActionResult Create(int? id)
        {            
            return View();
        }
        

        // POST: Inventarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = bindCategoriaActivo)] CategoriaDeActivo CategoriaDeActivo)
        {
            if (ModelState.IsValid)
            {            
                db.CategoriasDeActivos.Add(CategoriaDeActivo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            //ViewBag.idSucursal = new SelectList(db.Sucursales, "idSucursal", "nombreSucursal", CategoriaDeActivo.departamentoID);
            return View(CategoriaDeActivo);
        }

        // GET: Inventarios/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaDeActivo CategoriaDeActivo = await db.CategoriasDeActivos.FindAsync(id);
            if (CategoriaDeActivo == null)
            {
                return HttpNotFound();
            }            
            return View(CategoriaDeActivo);
        }

        // POST: Inventarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = bindCategoriaActivo )] CategoriaDeActivo CategoriaDeActivo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(CategoriaDeActivo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }            
            return View(CategoriaDeActivo);
        }

        // GET: Inventarios/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaDeActivo CategoriaDeActivo = await db.CategoriasDeActivos.FindAsync(id);
            if (CategoriaDeActivo == null)
            {
                return HttpNotFound();
            }
            return View(CategoriaDeActivo);
        }

        // POST: Inventarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CategoriaDeActivo CategoriaDeActivo = await db.CategoriasDeActivos.FindAsync(id);
            db.CategoriasDeActivos.Remove(CategoriaDeActivo);
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
