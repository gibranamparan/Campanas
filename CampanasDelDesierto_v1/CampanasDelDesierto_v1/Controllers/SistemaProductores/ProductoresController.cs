using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;
using CampanasDelDesierto_v1.HerramientasGenerales;
using CampanasDelDesierto_v1.Models.SistemaProductores;

namespace CampanasDelDesierto_v1.Controllers
{
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class ProductoresController : Controller
    {
        private const string BIND_FIELDS = "idProductor,numProductor,nombreProductor,domicilio," +
            "fechaIntegracion,RFC,zona,nombreCheque,poblacion,telefono,nombreRepresentanteLegal";
        private const string ERRORMSG_NUMPRODUCTOR = "El numero de productor ingresado ya se encuentra registrado a {0}," +
                    " ingrese uno diferente porfavor.";
        private const string ERRORMSG_FECHA_ADEUDOS_INICIALES = "No es posible agregar deudas anteriores si no existe al menos una temporada " +
                    "antes que incluya la fecha de integración registrada para el productor. Si desea ingresar adeudos iniciales para este productor, " +
                    "modifique la fecha de integración por una que entre dentor de las temporadas registradas o bien, " +
                    "cree una temporada que incluya dicha fecha.";

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Productores
        public ActionResult Index()
        {
            var productores = db.Productores.Where(pro => pro.Desactivado == false).ToList();
            return View(productores);
        }
        //Vista de ususarios desactivados
        public ActionResult Desactivados()
        {
            var productores = db.Productores.Where(pro => pro.Desactivado == true).ToList();
            return View(productores); 
        }

        //Accion para desactivar a un usuario
        public ActionResult Desactivar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Productor productor = db.Productores.Find(id);
            if (productor == null)
            {
                return HttpNotFound();
            }

            productor.Desactivado = true;
            db.SaveChanges();
            return RedirectToAction("Desactivados");
        }

        //Accion para desactivar a un usuario
        public ActionResult Activar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Productor productor = db.Productores.Find(id);
            if (productor == null)
            {
                return HttpNotFound();
            }

            productor.Desactivado = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Post: INDEX
        [HttpPost, ActionName("Index")]
        public ActionResult ImportFromExcel(HttpPostedFileBase xlsFile)
        {
            //Lista para recoleccion de errores
            List<ExcelTools.ExcelParseError> errores = new List<ExcelTools.ExcelParseError>();
            ExcelTools.ExcelParseError errorGeneral = new ExcelTools.ExcelParseError();
            //Se importan los datos de recepcion de producto desde el excel recibido
            int regsSaved = Productor.importarProductores(xlsFile, db, out errores, out errorGeneral);
            
            if(errores.Count()>0)
                ViewBag.erroresExcel = errores;
            if(errorGeneral.isError)
                ViewBag.errorGeneral = errorGeneral;

            return View(db.Productores.ToList());
        }

        // GET: Productores/Details/5
        public ActionResult Details(int? id, int? temporada)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Productor productor = db.Productores.Find(id);
            if (productor == null)
            {
                return HttpNotFound();
            }

            TemporadaDeCosecha tem = TemporadaDeCosecha.findTemporada(temporada);
            
            ViewBag.temporadas = db.TemporadaDeCosechas.OrderByDescending(temp=> temp.fechaFin).ToList();
            ViewBag.temporada = tem.TemporadaDeCosechaID;
            ViewBag.temporadaSeleccionada = tem;

            ViewBag.reporteMovimientos = productor.generarReporteAnticiposConIntereses(DateTime.Today, tem, db);

            return View(productor);
        }

        // GET: Productores/Create
        public ActionResult Create()
        {
            Productor prod = new Productor();
            prod.fechaIntegracion = DateTime.Today;
            return View(prod);
        }

        // POST: Productores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = BIND_FIELDS)] Productor productor, 
            AdeudoInicial adeudoAnticipos, AdeudoInicial adeudoArboles, AdeudoInicial adeudoMateriales)
        {
            //Se determina si el numero de productor ingresado ya corresponde a un productor guardado
            var prodBuscado = db.Productores.FirstOrDefault(prod => prod.numProductor == productor.numProductor);
            bool numProdYaExiste = prodBuscado!=null && prodBuscado.numProductor.ToUpper() == prodBuscado.numProductor.ToUpper();
            if (numProdYaExiste)
                ModelState.AddModelError("", String.Format(ERRORMSG_NUMPRODUCTOR, prodBuscado.nombreProductor));

            //Se determina la temporada de cosecha sobre la cual se comenzara a contar la deuda y se empezara el balance del productor
            TemporadaDeCosecha temporadaRegistro = TemporadaDeCosecha.findTemporada(db,productor.fechaIntegracion);
            bool hayAdeudoAnterior = adeudoAnticipos.montoActivo > 0 || adeudoArboles.montoActivo > 0;
            bool noHayTemporada = temporadaRegistro == null && hayAdeudoAnterior;
            if (noHayTemporada)
                ModelState.AddModelError("", ERRORMSG_FECHA_ADEUDOS_INICIALES);

            //Es valido el registro si sus datos generales lo son, no hay un productor con el mismo numero y 
            //en caso de haber sido introducido un adeudo, haya ademas registrada al menos una temporada en el sistema
            if (ModelState.IsValid && !numProdYaExiste && !noHayTemporada)
            {
                //Se establece una fecha inicial a los movimientos para garantizar que sean los 1ros en el balance
                DateTime fechaInicial;
                fechaInicial = temporadaRegistro.fechaInicio;

                //Se asocia a la fecha de integracion y temporada correspondiente, asi como al productor
                adeudoAnticipos.ajustarMovimiento(productor, temporadaRegistro, MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS);
                adeudoMateriales.ajustarMovimiento(productor, temporadaRegistro, MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS);
                adeudoAnticipos.ajustarMovimiento(productor, temporadaRegistro, MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO);
                adeudoMateriales.isVentas = true;
                adeudoMateriales.fechaMovimiento = adeudoAnticipos.fechaMovimiento.AddSeconds(1);
                adeudoArboles.fechaMovimiento = adeudoMateriales.fechaMovimiento.AddSeconds(1);

                //Se asocian los adeudos inciales al productor
                productor.MovimientosFinancieros = new List<MovimientoFinanciero>();
                if(Math.Abs(adeudoAnticipos.montoMovimiento)>0)
                    productor.MovimientosFinancieros.Add(adeudoAnticipos);
                if (Math.Abs(adeudoArboles.montoMovimiento) > 0)
                    productor.MovimientosFinancieros.Add(adeudoArboles);
                if (Math.Abs(adeudoArboles.montoMovimiento) > 0)
                    productor.MovimientosFinancieros.Add(adeudoMateriales);

                //Se hacen ajustes generales
                productor.RFC = productor.RFC.ToUpper();
                productor.nombreCheque = productor.nombreCheque.ToUpper();
                productor.nombreProductor = productor.nombreProductor.ToUpper();
                productor.numProductor = productor.numProductor.Trim();

                //Se guarda nuevo productor
                db.Productores.Add(productor);
                int numRegs = db.SaveChanges();
                return RedirectToAction("Details", "Productores", new { id = productor.idProductor });
            }

            return View(productor);
        }

        // GET: Productores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Productor productor = db.Productores.Find(id);
            if (productor == null)
            {
                return HttpNotFound();
            }
            return View("Create",productor);
        }

        // POST: Productores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = BIND_FIELDS)] Productor productor,
            AdeudoInicial adeudoAnticipos, AdeudoInicial adeudoArboles, AdeudoInicial adeudoMateriales)
        {
            //Se determina si el numero de productor ingresado ya corresponde a un productor guardado
            var prodBuscado = db.Productores.FirstOrDefault(prod => prod.numProductor == productor.numProductor);
            bool numProdYaExiste = prodBuscado != null && prodBuscado.idProductor != prodBuscado.idProductor
                && prodBuscado.numProductor.ToUpper() == prodBuscado.numProductor.ToUpper();
            db.Entry(prodBuscado).State = EntityState.Detached;

            if (numProdYaExiste) //Si ya existen un productor con el mismo numero
                ModelState.AddModelError("", String.Format(ERRORMSG_NUMPRODUCTOR, 
                        prodBuscado.nombreProductor));

            //Se determina la temporada de cosecha sobre la cual se comenzara a contar la deuda y se empezara el balance del productor
            TemporadaDeCosecha temporadaRegistro = TemporadaDeCosecha.findTemporada(db, productor.fechaIntegracion);
            bool hayAdeudoAnterior = adeudoAnticipos.montoActivo > 0 || adeudoArboles.montoActivo > 0;
            bool noHayTemporada = temporadaRegistro == null && hayAdeudoAnterior;
            if (noHayTemporada)
                ModelState.AddModelError("", ERRORMSG_FECHA_ADEUDOS_INICIALES);

            //Es valido el registro si sus datos generales lo son, no hay un productor con el mismo numero y 
            //en caso de haber sido introducido un adeudo, haya ademas registrada al menos una temporada en el sistema
            if (ModelState.IsValid && !numProdYaExiste && !noHayTemporada)
            {
                //Se hacen ajustes generales
                productor.RFC = productor.RFC==null?string.Empty:productor.RFC.ToUpper();
                productor.nombreCheque = productor.nombreCheque == null ? string.Empty : productor.nombreCheque.ToUpper();
                productor.nombreProductor = productor.nombreProductor==null?string.Empty: productor.nombreProductor.ToUpper();
                productor.numProductor = productor.numProductor.Trim();

                //Se asocia a la fecha de integracion y temporada correspondiente, asi como al productor
                adeudoAnticipos.ajustarMovimiento(productor, temporadaRegistro, MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS);
                adeudoMateriales.ajustarMovimiento(productor, temporadaRegistro, MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS);
                adeudoArboles.ajustarMovimiento(productor, temporadaRegistro, MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO);
                adeudoMateriales.fechaMovimiento = adeudoAnticipos.fechaMovimiento.AddSeconds(1);
                adeudoArboles.fechaMovimiento = adeudoMateriales.fechaMovimiento.AddSeconds(1);

                //Si el registro ya existia y el monto es cero, se elimina, si el monto no es cero
                db.Entry(adeudoAnticipos).State = AdeudoInicial.determinarEstadoMovimiento(adeudoAnticipos);
                db.Entry(adeudoMateriales).State = AdeudoInicial.determinarEstadoMovimiento(adeudoMateriales);
                db.Entry(adeudoArboles).State = AdeudoInicial.determinarEstadoMovimiento(adeudoArboles);
                adeudoMateriales.isVentas = true;

                db.Entry(productor).State = EntityState.Modified;
                int numRegs = db.SaveChanges();
                if (numRegs > 0)
                {
                    db.Entry(productor).Collection(prod => prod.MovimientosFinancieros).Load();
                    productor.restaurarDistribuciones(db);

                    productor.ajustarBalances(null, db, MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS);
                    productor.ajustarBalances(null, db, MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO);
                }
                return RedirectToAction("Details", new { id = productor.idProductor,
                    temporada = temporadaRegistro.TemporadaDeCosechaID});
            }
            return View("Create",productor);
        }

        // GET: Productores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Productor productor = db.Productores.Find(id);
            if (productor == null)
            {
                return HttpNotFound();
            }
            return View(productor);
        }

        [AllowAnonymous]
        public JsonResult restaurarDistribuciones(int id)
        {
            var prod = db.Productores.Find(id);
            int numRegs = prod.restaurarDistribuciones(db);
            return Json(new { registrosModificados = numRegs }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ajustaBalancesAnticipos(int id)
        {
            var prod = db.Productores.Find(id);
            int numRegs = prod.ajustarBalances(null,db,MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS);
            return Json(new { registrosModificados = numRegs }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult restaurarDistribucionesYBalancesParaTodos()
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
            return Json(new
            {
                numRegsDistribuciones = numRegsDistribuciones,
                numRegsBalances = numRegsBalances
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult restaurarDistribucionesYBalances(int id)
        {
            var prod = db.Productores.Find(id);
            int numRegsDistribuciones = 0;
            int numRegsBalances = 0;
            numRegsDistribuciones += prod.restaurarDistribuciones(db);
            numRegsBalances += prod.ajustarBalances(null, db, MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS);
            numRegsBalances += prod.ajustarBalances(null, db, MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO);

            return Json(new
            {
                numRegsDistribuciones = numRegsDistribuciones,
                numRegsBalances = numRegsBalances
            }, JsonRequestBehavior.AllowGet);
        }

        // POST: Productores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Productor productor = db.Productores.Find(id);
            db.Productores.Remove(productor);
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

        [AllowAnonymous]
        public ActionResult borrarSinIngresos()
        {
            var productores = db.Productores.Where(prod => prod.recepcionesDeProducto.Count() == 0);
            db.Productores.RemoveRange(productores);
            int numRegs = db.SaveChanges();
            return Json(new { registrosEliminados = numRegs });
        }
    }
}
