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
using System.Web.Script.Serialization;

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
        RecepcionDeProducto recepcionDeProducto, int recepcionID=0)
        {
            int numReg = 0;
            //Se verifica si el numero de recibo ya existe
            var reciboTemp = db.RecepcionesDeProducto.FirstOrDefault(mov => mov.numeroRecibo == recepcionDeProducto.numeroRecibo);
            bool numReciboExiste = reciboTemp != null && recepcionID==0; //Si se esta intentando crear un nuevo recibo con numero repetido

            //Si el modelo es valido y el numero de recibo no existe ya en la base de datos
            if (ModelState.IsValid && !numReciboExiste)
            {
                //Se introduce la informacion del productor indicado dentro del recibo que se encuentra registrando
                var productor = db.Productores.Find(recepcionDeProducto.idProductor);
                recepcionDeProducto.numProductor = productor!=null?productor.numProductor:"";
                recepcionDeProducto.nombreProductor = productor != null ? productor.nombreProductor:"";
                recepcionDeProducto.recepcionID = recepcionID;
                if (recepcionDeProducto.recepcionID == 0) //Si el recibo no existia, se marca para su creacion
                    db.RecepcionesDeProducto.Add(recepcionDeProducto);
                else //Si el recibo ya existia, se marca para ser editado
                {
                    var recTemp = db.RecepcionesDeProducto.Find(recepcionDeProducto.recepcionID);
                    recepcionDeProducto.importadoDesdeExcel = recTemp.importadoDesdeExcel;
                    db.Entry(recTemp).State = EntityState.Detached;
                    db.Entry(recepcionDeProducto).State = EntityState.Modified;
                }
                numReg = db.SaveChanges(); //Guarda cambios
                return Json(new { numReg = numReg, registro = recepcionDeProducto.ToString() }); //Responde con numero positivo (success)
            }else if (numReciboExiste) //Si tento registrar un nuevo recibo con numero ya existente
            {
                db.Entry(reciboTemp).State = EntityState.Detached;
                return Json(new
                {
                    numReg = 0,
                    error = "El numero de recibo ya existe",
                    registro = reciboTemp.ToString(),
                });
            }

            //El modelo no fue válido
            return Json(new { numReg = 0, error = "Favor de rellenar todos los campos." });
        }

        // POST: RecepcionDeProductos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string selectedIngresos, int TemporadaDeCosechaID, int? idProductor)
        {
            //int[] ingresosID = HerramientasGenerales.StringTools.jsonStringToArray(selectedIngresos);
            //Se deserializa la lista de compras en un objeto
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<int> ingresosID = js.Deserialize<List<int>>(selectedIngresos);

            int numReg = 0;
            foreach (int id in ingresosID)
            {
                var recepcionDeProducto = db.RecepcionesDeProducto.Find(id);
                db.RecepcionesDeProducto.Remove(recepcionDeProducto);
            }
            numReg = db.SaveChanges();
            if (idProductor != null) { 
                return RedirectToAction("IngresoProducto","PagosPorProductos", 
                    new { id= idProductor, temporada = TemporadaDeCosechaID });
            }else
            {
                return RedirectToAction("Details", "TemporadaDeCosechas",
                    new { id = TemporadaDeCosechaID });
            }
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
