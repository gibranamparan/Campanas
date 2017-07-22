using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;

namespace CampanasDelDesierto_v1.Controllers
{
    /// <summary>
    /// Representa las entidades de registro de cosecha de un productor.
    /// </summary>
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class PagosPorProductosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PagosPorProductos
        public ActionResult Index()
        {
            var pagosPorProductos = db.PagosPorProductos.Include(p => p.Productor);
            return View(pagosPorProductos.ToList());
        }

        // GET: PagosPorProductos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PagoPorProducto pagoPorProducto = db.PagosPorProductos.Find(id);
            if (pagoPorProducto == null)
            {
                return HttpNotFound();
            }
            return View(pagoPorProducto);
        }

        // GET: PagosPorProductos/Create
        public ActionResult Create(int? id = 0, int? temporada=0)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var productor = db.Productores.Find(id);
            if (productor == null)
            {
                return HttpNotFound();
            }

            var mov = prepararVistaCrear(productor);

            mov.introducirMovimientoEnPeriodo(temporada,db);

            return View(mov);
        }

        private PagoPorProducto prepararVistaCrear(Productor productor)
        {
            ViewBag.productor = productor;
            PagoPorProducto mov = new PagoPorProducto();
            mov.idProductor = productor.idProductor;
            mov.fechaMovimiento = DateTime.Now;

            return mov;
        }

        // POST: PagosPorProductos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,"+
            "TemporadaDeCosechaID")]
            PagoPorProducto pagoPorProducto)
        {
            if (ModelState.IsValid)
            {
                //Ajuste de movimiento para entrar dentro de la lista de balances
                pagoPorProducto.ajustarMovimiento();

                //Guardar cambios
                db.MovimientosFinancieros.Add(pagoPorProducto);
                int numReg = db.SaveChanges();

                if (numReg > 0)
                {
                    return RedirectToAction("Details", "Productores", 
                        new { id = pagoPorProducto.idProductor, temporada = pagoPorProducto.TemporadaDeCosechaID });
                }
            }

            var mov = prepararVistaCrear(db.Productores.Find(pagoPorProducto.idProductor));

            return View(mov);
        }

        public ActionResult IngresoProducto(int? id = 0, int? temporada = 0)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var productor = db.Productores.Find(id);
            if (productor == null)
            {
                return HttpNotFound();
            }

            var mov = prepararVistaIngresoProducto(productor, temporada.Value);

            mov.introducirMovimientoEnPeriodo(temporada, db);

            return View(mov);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IngresoProducto(PagoPorProducto pagoPorProducto, string selectedIngresos)
        {
            int[] ingresosID = HerramientasGenerales.StringTools.jsonStringToArray(selectedIngresos);

            if (ModelState.IsValid)
            {
                //Ajuste de movimiento para entrar dentro de la lista de balances
                pagoPorProducto.ajustarMovimiento();

                //Pago por producto
                db.PagosPorProductos.Add(pagoPorProducto);
                int numReg = db.SaveChanges();

                if (numReg > 0)
                {
                    //Recepcion de productos
                    foreach(int id in ingresosID)
                    {
                        var recepcion = db.RecepcionesDeProducto.Find(id);
                        recepcion.movimientoID = pagoPorProducto.idMovimiento;
                        db.Entry(recepcion).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                    //Se calcula el movimiento anterior al que se esta registrando
                    var prod = db.Productores.Find(pagoPorProducto.idProductor);
                    MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(pagoPorProducto.fechaMovimiento,pagoPorProducto.tipoDeBalance);
                    //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                    prod.ajustarBalances(ultimoMovimiento, db, pagoPorProducto.tipoDeBalance);

                    return RedirectToAction("Details", "Productores",
                        new { id = pagoPorProducto.idProductor, temporada = pagoPorProducto.TemporadaDeCosechaID });
                }
            }

            var productor = db.Productores.Find(pagoPorProducto.idProductor);
            var mov = prepararVistaIngresoProducto(productor, pagoPorProducto.TemporadaDeCosechaID);
            mov.introducirMovimientoEnPeriodo(pagoPorProducto.TemporadaDeCosechaID, db);

            return View(mov);
        }

        private PagoPorProducto prepararVistaIngresoProducto(Productor productor, int temporadaID)
        {
            PagoPorProducto mov = prepararVistaCrear(productor);
            var ingresos = db.RecepcionesDeProducto.Where(rec => rec.numProductor == productor.numProductor
            && rec.TemporadaDeCosechaID == temporadaID && (rec.movimientoID == 0 || rec.movimientoID == null)).ToList();
            ViewBag.ingresosProducto = ingresos;
            return mov;
        }

        // GET: PagosPorProductos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ppp = db.PagosPorProductos.Find(id);

            if (ppp == null)
            {
                return HttpNotFound();
            }

            prepararVistaEditarPagoProducto(ppp);

            return View("IngresoProducto", ppp);
        }

        /// <summary>
        /// Prepara la vista de edicion de pago por producto
        /// </summary>
        /// <param name="ppp">Pago por producto en cuestion.</param>
        private void prepararVistaEditarPagoProducto(PagoPorProducto ppp)
        {
            //Para editar, deben de mostrarse todas recepciones de producto de esta temporada de este productor
            //y que hayan sido pagadas por el movimiento editado
            var ingresos = db.RecepcionesDeProducto.Where(rec => rec.numProductor == ppp.Productor.numProductor
            && rec.TemporadaDeCosechaID == ppp.TemporadaDeCosechaID && (rec.movimientoID == null || rec.movimientoID == 0 
            || rec.movimientoID == ppp.idMovimiento)).ToList();
            ViewBag.ingresosProducto = ingresos;
        }

        // POST: PagosPorProductos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PagoPorProducto pagoPorProducto, string selectedIngresos)
        {
            int[] ingresosID = HerramientasGenerales.StringTools.jsonStringToArray(selectedIngresos);
            var pagoTemp = db.PagosPorProductos.Find(pagoPorProducto.idMovimiento);
            bool yaLiquidado = pagoTemp.yaLiquidado;
            db.Entry(pagoTemp).State = EntityState.Detached;

            if (ModelState.IsValid && !yaLiquidado)
            {
                //Ajuste de movimiento para entrar dentro de la lista de balances
                pagoPorProducto.ajustarMovimiento();

                //Pago por producto
                db.Entry(pagoPorProducto).State = EntityState.Modified;
                int numReg = db.SaveChanges();

                if (numReg > 0)
                {
                    //Se desasocian todos los registros de recepcion que habia
                    var recepciones = db.RecepcionesDeProducto.Where(rec=>rec.movimientoID == pagoPorProducto.idMovimiento);
                    foreach (var rec in recepciones)
                    {
                        rec.movimientoID = null;
                        db.Entry(rec).State = EntityState.Modified;
                    }

                    //se asocian los nuevos
                    foreach (int id in ingresosID)
                    {
                        var recepcion = db.RecepcionesDeProducto.Find(id);
                        recepcion.movimientoID = pagoPorProducto.idMovimiento;
                        db.Entry(recepcion).State = EntityState.Modified;
                    }

                    int numRegs = db.SaveChanges();

                    return RedirectToAction("Details", "Productores",
                        new { id = pagoPorProducto.idProductor, temporada = pagoPorProducto.TemporadaDeCosechaID });
                }
            }

            var productor = db.Productores.Find(pagoPorProducto.idProductor);
            var mov = prepararVistaIngresoProducto(productor, pagoPorProducto.TemporadaDeCosechaID);

            return View(mov);
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
