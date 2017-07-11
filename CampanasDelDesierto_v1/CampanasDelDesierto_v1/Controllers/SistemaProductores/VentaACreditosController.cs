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
        public const string BIND_FORM  = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor," +
            "cantidadMaterial,TemporadaDeCosechaID,conceptoDeVenta,pagareVenta,ordenCompra";

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
        public ActionResult Create([Bind(Include = BIND_FORM)]
            VentaACredito ventaACredito, string  compras)
        {
            if (ModelState.IsValid)
            {
                ////se ejecuta el metodo de juste para calcular automaticamente el total de la venta 
                ventaACredito.ajustarMovimiento();

                //Se deserializa la lista de compras en un objeto
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<CompraProducto> comprasList = js.Deserialize<List<CompraProducto>>(compras);

                //Se asociacian las compras a la venta
                ventaACredito.ComprasProductos = comprasList;
                db.MovimientosFinancieros.Add(ventaACredito);

                //Se guardan cambios
                int numReg = db.SaveChanges();
                if (numReg > 0)
                {
                    ventaACredito.ComprasProductos.ToList().ForEach(com => db.Entry(com).Reference(p => p.producto).Load());
                    //Se calcula el movimiento anterior al que se esta registrando
                    var prod = db.Productores.Find(ventaACredito.idProductor);
                    MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(ventaACredito.fechaMovimiento,ventaACredito.tipoDeBalance);

                    //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                    prod.ajustarBalances(ultimoMovimiento, db,ventaACredito.tipoDeBalance);

                    if (ventaACredito.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS)
                        prod.asociarAbonosConPrestamos(db, ventaACredito);

                    return RedirectToAction("Details", "MovimientoFinancieros", new { id = ventaACredito.idMovimiento });
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
        public ActionResult Edit([Bind(Include = BIND_FORM)] VentaACredito ventaACredito, string compras)
        {
            if (ModelState.IsValid)
            {
                //Se eliminan las compras hechas anteriormente
                db.ComprasProductos.RemoveRange(db.ComprasProductos
                    .Where(com=>com.idMovimiento == ventaACredito.idMovimiento));
                db.SaveChanges();
                
                //Se deserializa la lista de compras nuevas en un objeto
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<CompraProducto> comprasList = js.Deserialize<List<CompraProducto>>(compras);

                //Se asocia con la llave foranea de idMovimiento (la de la venta) a la compra
                comprasList.ForEach(com => com.idMovimiento = ventaACredito.idMovimiento);
                comprasList.ForEach(com => db.Entry(com).State = EntityState.Added);

                //Se ajusta el movimiento mantiendo la hora de su registro aunque el dia haya sido modificado
                var movTemp = db.MovimientosFinancieros.Find(ventaACredito.idMovimiento);
                db.Entry(movTemp).State = EntityState.Detached;

                //se ejecuta el metodo de juste para calcular automaticamente el total de la venta 
                ventaACredito.ajustarMovimiento(movTemp.fechaMovimiento);
                //Se modifica el registro
                db.Entry(ventaACredito).State = EntityState.Modified;

                //Se guardan cambios
                int numReg = db.SaveChanges();
                if (numReg > 0)
                {
                    db.Entry(ventaACredito).Collection(vc => vc.ComprasProductos).Load();
                    ventaACredito.ComprasProductos.ToList().ForEach(com => db.Entry(com).Reference(p => p.producto).Load());

                    //Se calcula el movimiento anterior al que se esta registrando
                    var prod = db.Productores.Find(ventaACredito.idProductor);
                    MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(ventaACredito.fechaMovimiento, ventaACredito.tipoDeBalance);

                    //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                    prod.ajustarBalances(ultimoMovimiento, db, ventaACredito.tipoDeBalance);

                    if (ventaACredito.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS)
                        prod.asociarAbonosConPrestamos(db, ventaACredito,true);

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
