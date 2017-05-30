using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;
using static CampanasDelDesierto_v1.HerramientasGenerales.FiltrosDeSolicitudes;

namespace CampanasDelDesierto_v1.Controllers
{
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class RecepcionDeProductosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // POST: RecepcionDeProductos
        /// <summary>
        /// Arrojar recepciones de producto para vistas para crear nuevo reporte
        /// </summary>
        /// <param name="numProductor"></param>
        /// <param name="temporadaCosechaID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RecepcionesSinReportar(string numProductor, int temporadaCosechaID)
        {
            var recepcionesDeProducto = from rec in (db.RecepcionesDeProducto
                                        .Where(rec => rec.numProductor == numProductor
                                            && rec.TemporadaDeCosechaID == temporadaCosechaID
                                            && (rec.movimientoID == 0 || rec.movimientoID == null)).ToList())
                                        select new RecepcionDeProducto.VMRecepcionProducto(rec);

            return Json(new { response = new  { data = recepcionesDeProducto.ToList()} });
        }

        // GET: RecepcionDeProductos para editar reporte
        public JsonResult RecepcionesParaEditarReporte(PagoPorProducto ppp)
        {
            var recepcionesDeProducto = from rec in (db.RecepcionesDeProducto
                                        .Where(rec => rec.numProductor == ppp.Productor.numProductor
                                            && rec.TemporadaDeCosechaID == ppp.TemporadaDeCosechaID 
                                            && (rec.movimientoID == null || rec.movimientoID == 0
                                            || rec.movimientoID == ppp.idMovimiento)).ToList())
                                        select new RecepcionDeProducto.VMRecepcionProducto(rec);

            return Json(recepcionesDeProducto.ToList());
        }
        // POST: RecepcionDeProductos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public JsonResult Create([Bind(Include = "numeroRecibo,numProductor,cantidadTonsProd1,"+
            "cantidadTonsProd2,cantidadTonsProd3,fecha,semana,TemporadaDeCosechaID,idProductor")]
        RecepcionDeProducto recepcionDeProducto)
        {
            int numReg = 0;
            if (ModelState.IsValid)
            {
                db.RecepcionesDeProducto.Add(recepcionDeProducto);
                numReg = db.SaveChanges();
                return Json(new { numReg = numReg, registro = recepcionDeProducto });
            }

            return Json(new { numReg = 0, error = "Favor de rellenar todos los campos." });
        }
        /*

        // POST: RecepcionDeProductos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit([Bind(Include = "recepcionID,numeroRecibo,numProductor,nombreProductor,cantidadTonsProd1,cantidadTonsProd2,cantidadTonsProd3,fecha,semana,TemporadaDeCosechaID,idProductor,movimientoID")] RecepcionDeProducto recepcionDeProducto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recepcionDeProducto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.movimientoID = new SelectList(db.MovimientosFinancieros, "idMovimiento", "idMovimiento", recepcionDeProducto.movimientoID);
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "numProductor", recepcionDeProducto.idProductor);
            ViewBag.TemporadaDeCosechaID = new SelectList(db.TemporadaDeCosechas, "TemporadaDeCosechaID", "TemporadaDeCosechaID", recepcionDeProducto.TemporadaDeCosechaID);
            return View(recepcionDeProducto);
        }
        
        */
        // POST: RecepcionDeProductos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string selectedIngresos, int idProductor, int TemporadaDeCosechaID)
        {
            int[] ingresosID = HerramientasGenerales.StringTools.jsonStringToArray(selectedIngresos);
            int numReg = 0;
            foreach (int id in ingresosID)
            {
                var recepcionDeProducto = db.RecepcionesDeProducto.Find(id);
                db.RecepcionesDeProducto.Remove(recepcionDeProducto);
            }
            numReg = db.SaveChanges();
            return RedirectToAction("IngresoProducto","PagosPorProductos", 
                new { id= idProductor, temporada = TemporadaDeCosechaID });
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
