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
    public class EmpleadosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Empleados
        public async Task<ActionResult> Index()
        {
            var empleados = db.Empleados.Include(e => e.Departamento);
            ViewBag.departamentoID = new SelectList(db.Departamentos.ToList(), "departamentoID", "nombreDepartamento");
            return View(await empleados.ToListAsync());
        }

        // GET: Empleados/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado empleado = await db.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ActivosResguardo = new SelectList(db.AdquisiscionDeActivos.Where(ac => ac.prestamo.idEmpleado == empleado.idEmpleado)
            //    .ToList(), "");
            return View(empleado);
        }

        // GET: Empleados/Create
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
            Empleado emp = prepararVistaCrear(Departamento);            

            return View(emp);
        }

        private Empleado prepararVistaCrear(Departamento Departamento)
        {
            ViewBag.Departamento = Departamento;
            ViewBag.departamentoID = new SelectList(db.Departamentos.ToList(), "departamentoID", "nombreDepartamento", null);

            Empleado emp = new Empleado();
            emp.departamentoID = Departamento.departamentoID;
            //emp.Departamento.nombreDepartamento = departamento.nombreDepartamento;
            //emp.Departamento.domicilio = departamento.domicilio;
            //emp.Departamento.Sucursal.nombreSucursal = departamento.Sucursal.nombreSucursal;
            emp.Departamento = Departamento;
            
            

            return emp;
        }

        // POST: Empleados/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "idEmpleado,nombre,apellidoPaterno,apellidoMaterno,departamentoID")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                db.Empleados.Add(empleado);
                await db.SaveChangesAsync();
                return RedirectToAction("Details/"+empleado.departamentoID, "Departamentos");
            }

            ViewBag.departamentoID = new SelectList(db.Departamentos, "departamentoID", "nombreDepartamento", empleado.departamentoID);
            return View(empleado);
        }

        // GET: Empleados/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado empleado = await db.Empleados.FindAsync(id);
            Departamento Departamento = empleado.Departamento;

            if (Departamento == null || empleado==null)
            {
                return HttpNotFound();
            }

            empleado = prepararVistaCrear(Departamento);                                  
            ViewBag.departamentoID = new SelectList(db.Departamentos, "departamentoID", "nombreDepartamento", empleado.departamentoID);
            return View(empleado);
        }

        // POST: Empleados/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "idEmpleado,nombre,apellidoPaterno,apellidoMaterno,departamentoID")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empleado).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details/"+empleado.departamentoID, "Departamentos");
            }
            ViewBag.departamentoID = new SelectList(db.Departamentos, "departamentoID", "nombreDepartamento", empleado.departamentoID);
            return View(empleado);
        }

        // GET: Empleados/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado empleado = await db.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            return View(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Empleado empleado = await db.Empleados.FindAsync(id);
            db.Empleados.Remove(empleado);
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
