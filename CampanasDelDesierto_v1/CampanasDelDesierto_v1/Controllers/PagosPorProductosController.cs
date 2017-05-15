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

            return mov;
        }

        // POST: PagosPorProductos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,"+
            "cantidadProducto,numeroSemana,tipoProducto,TemporadaDeCosechaID")]
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
                    //Se calcula el movimiento anterior al que se esta registrando
                    var prod = db.Productores.Find(pagoPorProducto.idProductor);
                    MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(pagoPorProducto.fechaMovimiento);
                    //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                    prod.ajustarBalances(ultimoMovimiento, db);

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

        private PagoPorProducto prepararVistaIngresoProducto(Productor productor, int temporadaID)
        {
            PagoPorProducto mov = prepararVistaCrear(productor);
            var ingresos = db.RecepcionesDeProducto.Where(rec => rec.numProductor == productor.numProductor
            && rec.TemporadaDeCosechaID == temporadaID).ToList();
            ViewBag.ingresosProducto = ingresos;
            return mov;
        }

        [HttpPost]
        public ActionResult IngresoProducto([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,"+
            "cantidadProducto,numeroSemana,tipoProducto,TemporadaDeCosechaID")] PagoPorProducto pagoPorProducto)
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
                    //Se calcula el movimiento anterior al que se esta registrando
                    var prod = db.Productores.Find(pagoPorProducto.idProductor);
                    MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(pagoPorProducto.fechaMovimiento);
                    //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                    prod.ajustarBalances(ultimoMovimiento, db);

                    return RedirectToAction("Details", "Productores",
                        new { id = pagoPorProducto.idProductor, temporada = pagoPorProducto.TemporadaDeCosechaID });
                }
            }
            var mov = prepararVistaIngresoProducto(db.Productores.Find(pagoPorProducto.idProductor), pagoPorProducto.TemporadaDeCosechaID);
            return View(mov);
        }

        // GET: PagosPorProductos/Edit/5
        public ActionResult Edit(int? id)
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

            ViewBag.productor = pagoPorProducto.Productor;

            return View(pagoPorProducto);
        }


        // POST: PagosPorProductos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,"+
            "cantidadProducto,numeroSemana,tipoProducto,TemporadaDeCosechaID")]
            PagoPorProducto pagoPorProducto)
        {
            if (ModelState.IsValid)
            {
                pagoPorProducto.ajustarMovimiento();
                db.Entry(pagoPorProducto).State = EntityState.Modified;
                int numreg = db.SaveChanges();

                if (numreg > 0)
                {
                    //Se calcula el movimiento anterior al que se esta registrando
                    var prod = db.Productores.Find(pagoPorProducto.idProductor);
                    MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(pagoPorProducto.fechaMovimiento);
                    //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                    prod.ajustarBalances(ultimoMovimiento, db);
                    return RedirectToAction("Details", "Productores", new { id = pagoPorProducto.idProductor,
                        temporada = pagoPorProducto.TemporadaDeCosechaID });
                }
            }

            ViewBag.productor = pagoPorProducto.Productor;

            return View(pagoPorProducto);
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
