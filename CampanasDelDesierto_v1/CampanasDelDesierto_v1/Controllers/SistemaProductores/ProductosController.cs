using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;
using System.Web.Helpers;
using CampanasDelDesierto_v1.HerramientasGenerales;
using static CampanasDelDesierto_v1.HerramientasGenerales.FiltrosDeSolicitudes;

namespace CampanasDelDesierto_v1.Controllers
{
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class ProductosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Activos
        public ActionResult Index()
        {
            return View(db.Productos.ToList());
        }

        // Post: INDEX
        [HttpPost, ActionName("Index")]
        public ActionResult ImportFromExcel(HttpPostedFileBase xlsFile)
        {
            //Lista para recoleccion de errores
            List<ExcelTools.ExcelParseError> errores = new List<ExcelTools.ExcelParseError>();
            ExcelTools.ExcelParseError errorGeneral = new ExcelTools.ExcelParseError();
            //Se importan los datos de recepcion de producto desde el excel recibido
            int regsSaved = Producto.importarProductores(xlsFile, db, out errores, out errorGeneral);

            if (errores.Count() > 0)
                ViewBag.erroresExcel = errores;
            if (errorGeneral.isError)
                ViewBag.errorGeneral = errorGeneral;

            return View(db.Productos.ToList());
        }

        // GET: Activos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
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
        public ActionResult Create([Bind(Include = "nombreProducto,costo,descripcion,concepto")]
            Producto producto, string UnidadMedida)
        {
            if (ModelState.IsValid)
            {
                producto.UnidadMedida = Producto.UnidadDeMedida.GetByName(UnidadMedida);
                db.Productos.Add(producto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(producto);
        }

        // GET: Activos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnidadMedida = producto.UnidadMedida.nombre;
            return View(producto);
        }

        // POST: Activos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idProducto,nombreProducto,costo,descripcion,concepto")]
            Producto producto, string UnidadMedida)
        {
            if (ModelState.IsValid)
            {
                producto.UnidadMedida = Producto.UnidadDeMedida.GetByName(UnidadMedida);
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(producto);
        }

        // GET: Activos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Activos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Producto producto = db.Productos.Find(id);
            db.Productos.Remove(producto);
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

        [HttpPost, ValidateHeaderAntiForgeryToken]
        public JsonResult productoInfo(int id=0)
        {
            Producto producto= new Producto();
            if (id != 0) 
                producto = db.Productos.Find(id);

            var vmProducto = new { id = id, nombre = producto.nombreProducto, costo = producto.costo };
            return Json(vmProducto);
        }

    }
}
