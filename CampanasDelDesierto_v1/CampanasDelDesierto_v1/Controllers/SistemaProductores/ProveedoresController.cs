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
using Proveedores = CampanasDelDesierto_v1.Models.PrestamoYAbonoCapital.Proveedores;
using Concepto = CampanasDelDesierto_v1.Models.PrestamoYAbonoCapital.Conceptos;

namespace CampanasDelDesierto_v1.Controllers
{
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class ProveedoresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Proveedores
        public ActionResult Index()
        {
            var conceptos = db.Conceptos;
            ViewBag.conceptos = conceptos.ToList();
            return View(db.Proveedores.ToList());
        }

       
        // GET: Proveedores/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nombreProveedor")] Proveedores proveedor)
        {
            if (ModelState.IsValid)
            {
                proveedor.nombreProveedor = proveedor.nombreProveedor.Trim().ToUpper();
                db.Proveedores.Add(proveedor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConcepto([Bind(Include = "id,nombreConcepto")] Concepto concepto)
        {
            if (ModelState.IsValid)
            {
                concepto.nombreConcepto = concepto.nombreConcepto.Trim().ToUpper();
                db.Conceptos.Add(concepto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        // POST: DeleteProveedor/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProveedor(int? id)
        {
            if(id==null || id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Proveedores proveedores = db.Proveedores.Find(id);
            if (proveedores == null)
                return HttpNotFound();

            db.Proveedores.Remove(proveedores);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: DeleteConcepto/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConcepto(int? id)
        {
            Concepto concepto = db.Conceptos.Find(id);
            db.Conceptos.Remove(concepto);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Conceptos()
        {
            return View(db.Conceptos.ToList());
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
