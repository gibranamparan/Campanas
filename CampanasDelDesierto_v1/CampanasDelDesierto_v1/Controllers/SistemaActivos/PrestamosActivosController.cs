﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;
using System.Web.Script.Serialization;

namespace CampanasDelDesierto_v1.Controllers
{
    [Authorize]
    public class PrestamosActivosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PrestamosActivos
        public ActionResult Index()
        {        
            var prestamoActivos = db.PrestamoActivos.Include(p => p.AdquisicionDeActivos).Include(p => p.Empleado).OrderBy(a=> a.fechaPrestamoActivo).Take(10);
            return View(prestamoActivos.ToList());
        }
        //Metodo post para poder realizar la busqueda (Buscador rango por fechas)
        [HttpPost]
        public ActionResult Index(DateTime fechaI, DateTime fechaF)
        {
            var prestamos = db.PrestamoActivos.Where(resultado => resultado.fechaPrestamoActivo>= fechaI && resultado.fechaPrestamoActivo<=fechaF).ToList();
            return View(prestamos);
        }

        // GET: PrestamosActivos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoActivo prestamoActivo = db.PrestamoActivos.Find(id);
            if (prestamoActivo == null)
            {
                return HttpNotFound();
            }
            return View(prestamoActivo);
        }

        // GET: PrestamosActivos/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado empleado = db.Empleados.Find(id);        
            if (empleado == null)
            {
                return HttpNotFound();

            }            
            ViewBag.ProductoActivoID = db.ProductosActivos.Where(ac=>ac.Activo.departamentoID == empleado.departamentoID).ToList();
            ViewBag.Empleado = db.Empleados.Find(id);
            
            //ViewBag.idActivo = new SelectList(db.Activos.ToList().Where(a=>a.prestado()==false), "idActivo", "nombreActivo", null);
            PrestamoActivo prestamo = prepararVistaCrear(empleado);            
            return View(prestamo);             
            

        }
        private PrestamoActivo prepararVistaCrear(Empleado empleado)
        {
            ViewBag.empleado = empleado;
            ViewBag.idEmpleado = new SelectList(db.Empleados.ToList(), "idEmpleado", "nombre", null);

            PrestamoActivo prestamo = new PrestamoActivo();
            prestamo.idEmpleado = empleado.idEmpleado;
            prestamo.Empleado = empleado;
            prestamo.fechaPrestamoActivo = DateTime.Today;
            prestamo.fechaEntregaActivo = DateTime.Today;


            return prestamo;
        }

        // POST: PrestamosActivos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idPrestamoActivo,fechaPrestamoActivo,fechaEntregaActivo,idEmpleado")] PrestamoActivo prestamoActivo, string Activos_AD)
        {
            if (ModelState.IsValid)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<AdquisicionDeActivo> ProductoActivosList = js.Deserialize<List<AdquisicionDeActivo>>(Activos_AD);               
                //Se asocian
                foreach (var productoActivo in ProductoActivosList)
                {                   
                    var activoEncontrado = db.ProductosActivos.Find(productoActivo.ProductoActivoID);
                    activoEncontrado.fechaPrestamo = DateTime.Now;   
                }
                prestamoActivo.AdquisicionDeActivos = ProductoActivosList;
                db.PrestamoActivos.Add(prestamoActivo);
                int numReg = db.SaveChanges();
                if (numReg > 0)
                {
                    //Se recargan las entidades de productos asociadass a cada compra dentro del prestamo
                    prestamoActivo.AdquisicionDeActivos.ToList().ForEach(com => db.Entry(com).Reference(p => p.ProductoActivo).Load());                    

                    return RedirectToAction("Details", "PrestamosActivos", new { id = prestamoActivo.idPrestamoActivo });
                }
                return RedirectToAction("Details", "PrestamosActivos", new { id = prestamoActivo.idPrestamoActivo });
            }     
      

            ViewBag.idActivo = new SelectList(db.Activos, "idActivo", "nombreActivo");
            ViewBag.idEmpleado = new SelectList(db.Empleados, "idEmpleado", "nombre", prestamoActivo.idEmpleado);
            return View(prestamoActivo);
        }

        // GET: PrestamosActivos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoActivo prestamoActivo = db.PrestamoActivos.Find(id);
            if (prestamoActivo == null)
            {
                return HttpNotFound();
            }
            ViewBag.idActivo = new SelectList(db.Activos, "idActivo", "nombreActivo");
            ViewBag.idEmpleado = new SelectList(db.Empleados, "idEmpleado", "nombre", prestamoActivo.idEmpleado);
            return View(prestamoActivo);
        }

        // POST: PrestamosActivos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idPrestamoActivo,fechaPrestamoActivo,fechaEntregaActivo,fechaDevolucion,idEmpleado,idActivo")] PrestamoActivo prestamoActivo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(prestamoActivo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idActivo = new SelectList(db.Activos, "idActivo", "nombreActivo");
            ViewBag.idEmpleado = new SelectList(db.Empleados, "idEmpleado", "nombre", prestamoActivo.idEmpleado);
            return View(prestamoActivo);
        }

        // GET: PrestamosActivos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoActivo prestamoActivo = db.PrestamoActivos.Find(id);
            if (prestamoActivo == null)
            {
                return HttpNotFound();
            }
            return View(prestamoActivo);
        }

        // POST: PrestamosActivos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PrestamoActivo prestamoActivo = db.PrestamoActivos.Find(id);
            db.PrestamoActivos.Remove(prestamoActivo);
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
