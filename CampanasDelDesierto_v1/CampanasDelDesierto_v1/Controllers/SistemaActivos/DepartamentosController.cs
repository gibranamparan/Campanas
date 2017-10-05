using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using CampanasDelDesierto_v1.Models;
using CampanasDelDesierto_v1.HerramientasGenerales;

namespace CampanasDelDesierto_v1.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    public class DepartamentosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Departamentos
        public async Task<ActionResult> Index()
        {
            ViewBag.idSucursal = new SelectList(db.Sucursales.ToList(), "idSucursal", "nombreSucursal", null);
            if (User.IsInRole(ApplicationUser.RoleNames.ADMIN)) {
                //Se muestra el listado de todos los departamentos si es admin general
                var departamentos = db.Departamentos.Include(d => d.Sucursal);
                return View(await departamentos.ToListAsync());
             } else if (User.IsInRole(ApplicationUser.RoleNames.DEPARTAMENTO)) {
                //Si es admin de departamento, se muestran los detalles de su departamento
                var userID = User.Identity.GetUserId();
                AdminDepartamento adminDep = db.AdminsDepartamentos.Find(userID);
                return RedirectToAction("Details", new { id = adminDep.departamentoID });
             }

            return View();
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
            ViewBag.Activos = db.Activos.ToList();
            return View(departamento);
        }

        // GET: Departamentos/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sucursal Sucursal = db.Sucursales.Find(id);
            if (Sucursal == null)
            {
                return HttpNotFound();
            }
            Departamento dep = prepararVistaCrear(Sucursal);

            return View(dep);
        }

        private Departamento prepararVistaCrear(Sucursal Sucursal)
        {
            ViewBag.Sucursal = Sucursal;
            ViewBag.idSucursal = new SelectList(db.Sucursales.ToList(), "idSucursal", "nombreSucursal", null);

            Departamento dep = new Departamento();
            dep.idSucursal = Sucursal.idSucursal;
            //emp.Departamento.nombreDepartamento = departamento.nombreDepartamento;
            //emp.Departamento.domicilio = departamento.domicilio;
            //emp.Departamento.Sucursal.nombreSucursal = departamento.Sucursal.nombreSucursal;
            dep.Sucursal = Sucursal;



            return dep;
        }


        // POST: Departamentos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "departamentoID,nombreDepartamento,idSucursal, domicilio")] Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                db.Departamentos.Add(departamento);
                await db.SaveChangesAsync();
                return RedirectToAction("Details/"+departamento.idSucursal, "Sucursales");
            }

            ViewBag.idSucursal = new SelectList(db.Sucursales, "idSucursal", "nombreSucursal", departamento.idSucursal);
            return View(departamento);
        }
        [HttpPost, ActionName("Details")]
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

            if (departamentoID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamento departamento =  db.Departamentos.Find(departamentoID);
            if (departamento == null)
            {
                return HttpNotFound();
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
        public async Task<ActionResult> Edit([Bind(Include = "departamentoID,nombreDepartamento,idSucursal,domicilio")] Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departamento).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details/"+departamento.Sucursal.idSucursal,"Sucursales");
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
        public ActionResult Inventarios(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamento Departamentos = db.Departamentos.Find(id);
            if (Departamentos == null)
            {
                return HttpNotFound();
            }

            return View(Departamentos);
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
