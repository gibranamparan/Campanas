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
        public const string BIND_FIELDS = "numeroRecibo,numProductor,cantidadTonsProd1," +
            "cantidadTonsProd2,cantidadTonsProd3,cantidadTonsProd4,cantidadTonsProd5,cantidadTonsProd6,"+
            "fecha,semana,TemporadaDeCosechaID,idProductor";
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
        public JsonResult Create([Bind(Include = BIND_FIELDS)]
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

        [HttpGet]
        public ActionResult borrarTodos(int? temporadaID = 0)
        {
            if (temporadaID == null || temporadaID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TemporadaDeCosecha temporadaDeCosecha = db.TemporadaDeCosechas.Find(temporadaID);
            if (temporadaDeCosecha == null)
            {
                return HttpNotFound();
            }

            //Se buscan todas las recepciones importadas desde excel de la temporada indicada que aun no hayan sido asociadas
            //con ningun pago.
            List<RecepcionDeProducto> recepciones = db.RecepcionesDeProducto.Where(rec => rec.TemporadaDeCosechaID == temporadaID)
                .Where(rec => rec.importadoDesdeExcel).Where(rec => rec.movimientoID == null || rec.movimientoID == 0).ToList();
            
            int numRegs = 0;
            if(recepciones.Count()> 0) { 
                db.RecepcionesDeProducto.RemoveRange(recepciones);
                numRegs = db.SaveChanges();
            }

            return RedirectToAction("Details", "TemporadaDeCosechas", new { id = temporadaID });
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
