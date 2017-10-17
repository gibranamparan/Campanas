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
using CampanasDelDesierto_v1.HerramientasGenerales;

namespace CampanasDelDesierto_v1.Controllers
{
    [Authorize]
    public class ActivosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Activos
        public async Task<ActionResult> Index()
        {
            var activos = db.Activos;
            ViewBag.departamentoID = new SelectList(db.Departamentos.ToList(), "departamentoID", "nombreDepartamento");
            return View(await activos.ToListAsync());
        }
        // GET: Activos
        [HttpPost]
        public ActionResult Index(int departamentoID)
        {
            
            if (departamentoID > 0)
            {                
                var departamentoEnc = db.Departamentos.Find(departamentoID);
                var activosPrestados = departamentoEnc.Activos.Where(ac => ac.isPrestado == true).ToList();                            
                ViewBag.departamentoID = new SelectList(db.Departamentos.ToList(), "departamentoID", "nombreDepartamento");
                return View(activosPrestados.ToList());
            }
            else
            {                
                var activosPrestados = db.Activos.Where(ac => ac.isPrestado == true).ToList();                  
                ViewBag.departamentoID = new SelectList(db.Departamentos.ToList(), "departamentoID", "nombreDepartamento");
                return View(activosPrestados.ToList());
            }    
            
            
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamento Departamento = db.Departamentos.Find(id);
            if (Departamento == null)
            {
                return HttpNotFound();
            }
          
           
            Activo ac = prepararVistaCrear(Departamento);
            ViewBag.CategoriaActivoID = new SelectList(db.CategoriasDeActivos.ToList(), "CategoriaDeActivoID", "nombreCategoria", null);
            return View(ac);                 
        }
        private Activo prepararVistaCrear(Departamento Departamento)
        {
            ViewBag.Departamento = Departamento;
            ViewBag.departamentoID = new SelectList(db.Departamentos.ToList(), "departamentoID", "nombreDepartamento", null);
            Activo ac = new Activo();
            ac.departamentoID = Departamento.departamentoID;
            ac.Departamento = Departamento;
            return ac;
        }

        [HttpPost]
        public ActionResult ImportFromExcel(HttpPostedFileBase xlsFile, int departamentoID)
        {
            //Lista para recoleccion de errores
            List<ExcelTools.ExcelParseError> errores = new List<ExcelTools.ExcelParseError>();
            ExcelTools.ExcelParseError errorGeneral = new ExcelTools.ExcelParseError();
            //Se importan los datos de recepcion de producto desde el excel recibido
            int regsSaved = Activo.importarActivos(xlsFile, db, out errores, out errorGeneral, departamentoID);

            if (errores.Count() > 0)
                ViewBag.erroresExcel = errores;
            if (errorGeneral.isError)
                ViewBag.errorGeneral = errorGeneral;

            return View(db.Activos.ToList());
        }

        // POST: Activos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idActivo,nombreActivo,estadoActivo,CategoriaActivoID")] Activo activo)
        {
            if (ModelState.IsValid)
            {
                db.Activos.Add(activo);
                await db.SaveChangesAsync();
                return RedirectToAction("Details/" + activo.CategoriaActivoID, "CategoriasDeActivos");
            }

            ViewBag.CategoriaActivoID = new SelectList(db.CategoriasDeActivos, "CategoriaActivoID", "nombreCategoria", activo.CategoriaActivoID);
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
            ViewBag.CategoriaActivoID = new SelectList(db.CategoriasDeActivos, "CategoriaActivoID", "nombreCategoria", activo.CategoriaActivoID);
            return View(activo);
        }

        // POST: Activos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idActivo,nombreActivo,estadoActivo,CategoriaActivoID")] Activo activo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details/"+activo.CategoriaActivoID, "CategoriasDeActivos");
            }
            ViewBag.CategoriaActivoID = new SelectList(db.CategoriasDeActivos, "CategoriaActivoID", "nombreCategoria", activo.CategoriaActivoID);
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
