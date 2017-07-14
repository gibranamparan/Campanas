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
            "fechaIntegracion,RFC,zona,nombreCheque,adeudoAnterior,poblacion,telefono,nombreRepresentanteLegal"+
            ",adeudoAnteriorAnticipos,adeudoAnteriorArboles";
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
            AdeudoInicial adeudoAnticipos, AdeudoInicial adeudoArboles)
        {
            var prodBuscado = db.Productores.FirstOrDefault(prod => prod.numProductor == productor.numProductor);
            bool numProdYaExiste = prodBuscado!=null && prodBuscado.numProductor == prodBuscado.numProductor;
            if (numProdYaExiste)
            {
                ModelState.AddModelError("", 
                    String.Format("El numero de productor ingresado ya se encuentra registrado a {0}, ingrese uno diferente porfavor.", 
                    prodBuscado.nombreProductor));
            }

            var temporadaMasVieja = db.TemporadaDeCosechas.OrderBy(tem => tem.fechaInicio).FirstOrDefault();
            bool hayAdeudoAnterior = adeudoAnticipos.montoActivo > 0 || adeudoArboles.montoActivo > 0;
            bool noHayTemporada = temporadaMasVieja == null && hayAdeudoAnterior;
            if (noHayTemporada)
                ModelState.AddModelError("", "No es posible agregar deudas anteriores si no existe al menos una temporada antes.");

            //Es valido el registro si sus datos generales lo son, no hay un productor con el mismo numero y 
            //en caso de haber sido introducido un adeudo, haya ademas registrada al menos una temporada en el sistema
            if (ModelState.IsValid && !numProdYaExiste && !noHayTemporada)
            {
                //Se determina el tipo de balance al que corresponde cada adeudo
                adeudoAnticipos.balanceAdeudado = MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS;
                adeudoArboles.balanceAdeudado = MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO;

                //Se establece una fecha inicial a los movimientos para garantizar que sean los 1ros en el balance
                DateTime fechaInicial;
                fechaInicial = temporadaMasVieja.fechaInicio;

                adeudoAnticipos.fechaMovimiento = fechaInicial;
                adeudoArboles.fechaMovimiento = fechaInicial;
                //Se introducen dentro de la temporada mas vieja
                adeudoAnticipos.TemporadaDeCosechaID = temporadaMasVieja.TemporadaDeCosechaID;
                adeudoArboles.TemporadaDeCosechaID = temporadaMasVieja.TemporadaDeCosechaID;

                //Se asocian los adeudos inciales al productor
                productor.MovimientosFinancieros = new List<MovimientoFinanciero>();
                if(adeudoAnticipos.montoMovimiento!=0)
                    productor.MovimientosFinancieros.Add(adeudoAnticipos);
                if (adeudoArboles.montoMovimiento != 0)
                    productor.MovimientosFinancieros.Add(adeudoArboles);

                //Se hacen ajustes generales
                productor.RFC = productor.RFC.ToUpper();
                productor.nombreCheque = productor.nombreCheque.ToUpper();
                productor.nombreProductor = productor.nombreProductor.ToUpper();
                productor.numProductor = productor.numProductor.Trim();

                //Se guarda nuevo productor
                db.Productores.Add(productor);
                db.SaveChanges();
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
        public ActionResult Edit([Bind(Include = BIND_FIELDS)]
                Productor productor)
        {
            if (ModelState.IsValid)
            {
                productor.numProductor = productor.numProductor.Trim();
                db.Entry(productor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = productor.idProductor});
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
        public JsonResult restaurarDistribucionesParaTodos()
        {
            var prods = db.Productores.ToList();
            int numRegsDistribuciones = 0;
            int numRegsBalances = 0;
            foreach (var prod in prods)
            {
                numRegsDistribuciones += prod.restaurarDistribuciones(db);
                numRegsBalances += prod.ajustarBalances(null,db,MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS);
            }
            return Json(new { numRegsDistribuciones = numRegsDistribuciones,
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
