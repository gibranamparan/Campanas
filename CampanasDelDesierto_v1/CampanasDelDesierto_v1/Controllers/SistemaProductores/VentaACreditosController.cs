using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CampanasDelDesierto_v1.Models;

namespace CampanasDelDesierto_v1.Controllers
{
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class VentaACreditosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: VentaACreditos
        public ActionResult Index()
        {
            //var movimientosFinancieros = db.MovimientosFinancieros.Include(v => v.Productor);
            var ventacredito = db.VentasACreditos.Include(v => v.Productor);
            return View(ventacredito.ToList());
        }

        // GET: VentaACreditos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VentaACredito ventaACredito = db.VentasACreditos.Find(id);
            if (ventaACredito == null)
            {
                return HttpNotFound();
            }
            return View(ventaACredito);
        }

        // GET: VentaACreditos/Create
        public ActionResult Create(int? id, int? temporada)
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
            VentaACredito mov = prepararVistaCrear(productor);
            mov.introducirMovimientoEnPeriodo(temporada);
            ViewBag.productos = db.Productos.ToList();

            return View(mov);
        }

        private VentaACredito prepararVistaCrear(Productor productor)
        {
            ViewBag.productor = productor;
            ViewBag.idProducto = new SelectList(db.Productos.ToList(), "idProducto", "nombreProducto",null);

            VentaACredito mov = new VentaACredito();
            mov.idProductor = productor.idProductor;
            mov.fechaMovimiento = DateTime.Today;

            return mov;
        }

        // POST: VentaACreditos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,"+
            "cantidadMaterial,TemporadaDeCosechaID,conceptoDeVenta,pagareVenta")]
            VentaACredito ventaACredito, string  compras)
        {
            if (ModelState.IsValid)
            {
                ////se ejecuta el metodo de juste para calcular automaticamente el total de la venta 
                ventaACredito.ajustarMovimiento();

                //Se deserializa la lista de compras en un objeto
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<CompraProducto> comprasList = js.Deserialize<List<CompraProducto>>(compras);
                foreach (var compra in comprasList)
                {
                    db.Entry(compra).State = EntityState.Added;
                }
                db.MovimientosFinancieros.Add(ventaACredito);
                int numReg = db.SaveChanges();
                if (numReg > 0)
                {
                    //Se calcula el movimiento anterior al que se esta registrando
                    var prod = db.Productores.Find(ventaACredito.idProductor);
                    MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(ventaACredito.fechaMovimiento);

                    //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                    prod.ajustarBalances(ultimoMovimiento, db);

                    return RedirectToAction("Details", "Productores", new { id = ventaACredito.idProductor,
                        temporada = ventaACredito.TemporadaDeCosechaID });
                }
            }
            VentaACredito mov = prepararVistaCrear(db.Productores.Find(ventaACredito.idProductor));

            return View(ventaACredito);
        }

        // GET: VentaACreditos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VentaACredito ventaACredito = db.VentasACreditos.Find(id);
            if (ventaACredito == null)
            {
                return HttpNotFound();
            }

            prepararVistaCrear(ventaACredito.Productor);
            ViewBag.productos = db.Productos.ToList();

            return View("Create",ventaACredito);
        }

        // POST: VentaACreditos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,cantidadMaterial"+
            ",idProducto,TemporadaDeCosechaID")]VentaACredito ventaACredito)
        {
            if (ModelState.IsValid)
            {
                //Producto producto = db.Productos.Find(ventaACredito.idProducto);
                ////se ejecuta el metodo de juste para calcular automaticamente el total de la venta 
                //ventaACredito.ajustarMovimiento(producto);

                //Se modifica el registro
                db.Entry(ventaACredito).State = EntityState.Modified;
                int numReg = db.SaveChanges();
                if (numReg > 0)
                {
                    //Se calcula el movimiento anterior al que se esta registrando
                    var prod = db.Productores.Find(ventaACredito.idProductor);
                    MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(ventaACredito.fechaMovimiento);

                    //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                    prod.ajustarBalances(ultimoMovimiento, db);

                    return RedirectToAction("Details", "Productores", 
                        new { id = ventaACredito.idProductor, temporada = ventaACredito.TemporadaDeCosechaID });
                }
            }

            prepararVistaCrear(db.Productores.Find(ventaACredito.idProductor));
            return View(ventaACredito);
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
