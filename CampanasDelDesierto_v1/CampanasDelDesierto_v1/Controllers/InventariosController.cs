﻿using System;
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
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Inventarios
        public async Task<ActionResult> Index()
        {
            var inventarios = db.Inventarios.Include(i => i.Departamento);           
            return View(await inventarios.ToListAsync());
        }

        // GET: Inventarios/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventario inventario = await db.Inventarios.FindAsync(id);
            if (inventario == null)
            {
                return HttpNotFound();
            }
            return View(inventario);
        }

        // GET: Inventarios/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var departamento = db.Departamentos.Find(id);
            if (departamento == null)
            {
                return HttpNotFound();

            }
            Inventario emp = prepararVistaCrear(departamento);
            return View(emp);

        }
        private Inventario prepararVistaCrear(Departamento departamento)
        {
            ViewBag.departamento = departamento;
            ViewBag.departamentoID = new SelectList(db.Departamentos.ToList(), "departamentoID", "nombreDepartamento", null);

            Inventario emp = new Inventario();
            emp.departamentoID = departamento.departamentoID;


            return emp;
        }

        // POST: Inventarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "inventarioID,nombreInventario,modeloInventario,costo,cantidad,departamentoID")] Inventario inventario)
        {
            if (ModelState.IsValid)
            {
                var activos = inventario.cantidad;
                for (int i = 0; i < activos ; i++)
                {
                    //Se crea una instancia al modelo de activo
                    Activo activo = new Activo();
                    //se le asignan los datos
                    activo.nombreActivo = inventario.nombreInventario;
                    activo.inventarioID = inventario.inventarioID;
                    activo.estadoActivo = "No establecido";
                    //Se guardan los datos
                    db.Activos.Add(activo);
                    
                }
                db.Inventarios.Add(inventario);
                await db.SaveChangesAsync();
                return RedirectToAction("Inventarios/"+inventario.departamentoID,"Departamentos",inventario.departamentoID);
            }

            ViewBag.departamentoID = new SelectList(db.Departamentos, "departamentoID", "nombreDepartamento", inventario.departamentoID);
            return View(inventario);
        }

        // GET: Inventarios/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventario inventario = await db.Inventarios.FindAsync(id);
            if (inventario == null)
            {
                return HttpNotFound();
            }
            ViewBag.idSucursal = new SelectList(db.Sucursales, "idSucursal", "nombreSucursal", inventario.departamentoID);
            return View(inventario);
        }

        // POST: Inventarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "inventarioID,nombreInventario,modeloInventario,costo,cantidad,idSucursal")] Inventario inventario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventario).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idSucursal = new SelectList(db.Sucursales, "idSucursal", "nombreSucursal", inventario.departamentoID);
            return View(inventario);
        }

        // GET: Inventarios/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventario inventario = await db.Inventarios.FindAsync(id);
            if (inventario == null)
            {
                return HttpNotFound();
            }
            return View(inventario);
        }

        // POST: Inventarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Inventario inventario = await db.Inventarios.FindAsync(id);
            db.Inventarios.Remove(inventario);
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
