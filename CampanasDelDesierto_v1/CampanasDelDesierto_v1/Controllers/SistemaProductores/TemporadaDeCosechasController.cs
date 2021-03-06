﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;
using CampanasDelDesierto_v1.HerramientasGenerales;
using System.Data.OleDb;
using OfficeOpenXml;

namespace CampanasDelDesierto_v1.Controllers
{
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class TemporadaDeCosechasController : Controller
    {
        private const string BIND_FIELDS = "TemporadaDeCosechaID,fechaInicio,fechaFin,precioProducto1," +
            "precioProducto2,precioProducto3,precioProducto4,precioProducto5,precioProducto6,"+
            "precioProducto7,precioProducto8,precioProducto9,precioProducto10,precioProducto11,precioProducto12";
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TemporadaDeCosechas
        public ActionResult Index()
        {
            return View(db.TemporadaDeCosechas.OrderByDescending(tem=>tem.fechaFin).ToList());
        }

        // GET: TemporadaDeCosechas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TemporadaDeCosecha temporadaDeCosecha = db.TemporadaDeCosechas.Find(id);
            if (temporadaDeCosecha == null)
            {
                return HttpNotFound();
            }

            var productores = db.Productores.ToList();
            var recepciones = temporadaDeCosecha.recepcionesDeProducto.ToList();
            recepciones.ForEach(rec =>
            {
                var productor = productores.SingleOrDefault(prod => prod.numProductor == rec.numProductor);
                rec.idProductor = productor == null ? 0 : productor.idProductor;
            });
            temporadaDeCosecha.recepcionesDeProducto = recepciones;
            ViewBag.productores = new SelectList(productores, "idProductor", "numProductor");
            return View(temporadaDeCosecha);
        }

        // GET: TemporadaDeCosechas/Details/5
        [HttpPost, ActionName("Details")]
        public ActionResult UploadExcelAcumuladosAceituna(HttpPostedFileBase xlsFile, int id=0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TemporadaDeCosecha temporadaDeCosecha = db.TemporadaDeCosechas.Find(id);
            if (temporadaDeCosecha == null)
            {
                return HttpNotFound();
            }
            //Lista para recoleccion de errores
            List<ExcelTools.ExcelParseError> errores = new List<ExcelTools.ExcelParseError>();
            ExcelTools.ExcelParseError errorPrecios = new ExcelTools.ExcelParseError();
            //Se importan los datos de recepcion de producto desde el excel recibido
            int regsSaved = temporadaDeCosecha.importarIngresoDeProductos(xlsFile,db, out errores, out errorPrecios);

            if (regsSaved == 0)
                ModelState.AddModelError("", "No fue posible importar el excel, compruebe su estructura.");

            ViewBag.erroresExcel = errores;
            ViewBag.errorPrecios = errorPrecios;
            return View(temporadaDeCosecha);
        }

        // GET: TemporadaDeCosechas/Create
        public ActionResult Create()
        {
            TemporadaDeCosecha tem = new TemporadaDeCosecha();
            return View(tem);
        }

        // POST: TemporadaDeCosechas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = BIND_FIELDS)]
            TemporadaDeCosecha temporadaDeCosecha)
        {
            if (ModelState.IsValid)
            {
                db.TemporadaDeCosechas.Add(temporadaDeCosecha);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(temporadaDeCosecha);
        }

        // GET: TemporadaDeCosechas/Edit/5
        public ActionResult Edit(int? id, int? pagoPorProductoID, int? productorID, int? temporadaID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TemporadaDeCosecha temporadaDeCosecha = db.TemporadaDeCosechas.Find(id);
            if (temporadaDeCosecha == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.pagoPorProductoID = pagoPorProductoID;
            ViewBag.productorID = productorID;
            ViewBag.temporadaID = temporadaID;

            return View("Create",temporadaDeCosecha);
        }

        // POST: TemporadaDeCosechas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = BIND_FIELDS)]
            TemporadaDeCosecha temporadaDeCosecha)
        {
            if (ModelState.IsValid)
            {
                db.Entry(temporadaDeCosecha).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Create", temporadaDeCosecha);
        }

        // GET: TemporadaDeCosechas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TemporadaDeCosecha temporadaDeCosecha = db.TemporadaDeCosechas.Find(id);
            if (temporadaDeCosecha == null)
            {
                return HttpNotFound();
            }
            return View(temporadaDeCosecha);
        }

        // POST: TemporadaDeCosechas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TemporadaDeCosecha temporadaDeCosecha = db.TemporadaDeCosechas.Find(id);
            var prestamosYAbonos = db.Prestamo_Abono.Where(mov => mov.prestamo.TemporadaDeCosechaID == id || mov.abono.TemporadaDeCosechaID == id);
            db.Prestamo_Abono.RemoveRange(prestamosYAbonos);
            db.TemporadaDeCosechas.Remove(temporadaDeCosecha);
            int numRegs = db.SaveChanges();
            if (numRegs > 0)
            {
                var prods = db.Productores.ToList();
                int numRegsDistribuciones = 0;
                int numRegsBalances = 0;
                foreach (var prod in prods)
                {
                    numRegsDistribuciones += prod.restaurarDistribuciones(db);
                    numRegsBalances += prod.ajustarBalances(null, db, MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS);
                    numRegsBalances += prod.ajustarBalances(null, db, MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO);
                }
            }
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
