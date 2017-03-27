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
        public ActionResult Create()
        {
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor");
            return View();
        }

        // POST: PagosPorProductos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,cantidadProducto,numeroSemana,cheque,MyProperty,tipoProducto,garantiaLimpieza")] PagoPorProducto pagoPorProducto)
        {
            double ultimoBalancePrestamosCapital = 0;
            double ultimoBalancePrestamosMaterial = 0;
            int  idActivo = 0;
            VentaACredito ventaCredito = new VentaACredito();
            PrestamoYAbonoCapital prestamoAbonoCapital = new PrestamoYAbonoCapital();
            if (ModelState.IsValid)
            {
                try
                {
                    var movimientosAscendentes = db.PrestamosYAbonosCapital.Where(mov => mov.idProductor == pagoPorProducto.idProductor).OrderByDescending(mov => mov.fechaMovimiento);
                    var ultimoMov = movimientosAscendentes.First();
                    ultimoBalancePrestamosCapital = ultimoMov.balance;
                    var movimientos = db.VentasACreditos.Where(mov => mov.idProductor == pagoPorProducto.idProductor).OrderByDescending(mov => mov.fechaMovimiento);
                    var ultimoMovimiento = movimientos.First();
                    ultimoBalancePrestamosMaterial = ultimoMovimiento.balance;

                    idActivo = movimientos.First().idActivos;
                }
                catch { }
                pagoPorProducto.balance = pagoPorProducto.montoMovimiento - (ultimoBalancePrestamosCapital + ultimoBalancePrestamosMaterial);
                
                //aal calcular el balance del pago por producto se crea un nuevo registro en prestamoyabonocapital para que asi se cancele 
                //la deuda o se acumule para el siguiente año

                prestamoAbonoCapital.montoMovimiento = 0;
                prestamoAbonoCapital.fechaMovimiento = pagoPorProducto.fechaMovimiento;
                prestamoAbonoCapital.idProductor = pagoPorProducto.idProductor;
                prestamoAbonoCapital.fechaPagar = pagoPorProducto.fechaMovimiento;
                prestamoAbonoCapital.nota = "se realizo un pagoporproducto";
                //se hace unaa validacion si la cantidad de la produccion fue suficiente para cubrir la deuda se cancela sino se abona y se sigue acumulando
                if(pagoPorProducto.montoMovimiento >= (ultimoBalancePrestamosCapital + ultimoBalancePrestamosMaterial))
                {
                    prestamoAbonoCapital.balance = 0;
                    ventaCredito.balance = 0;
                }
                else
                {
                    prestamoAbonoCapital.balance = pagoPorProducto.balance;
                    ventaCredito.balance = pagoPorProducto.balance;
                }

                prestamoAbonoCapital.concepto = "Pago por producto";

                ventaCredito.montoMovimiento = 0;
                ventaCredito.fechaMovimiento = pagoPorProducto.fechaMovimiento;
                ventaCredito.idProductor = pagoPorProducto.idProductor;
                ventaCredito.cantidadMaterial = 0;
                ventaCredito.idActivos = idActivo;



                db.MovimientosFinancieros.Add(pagoPorProducto);
                db.MovimientosFinancieros.Add(prestamoAbonoCapital);
                db.MovimientosFinancieros.Add(ventaCredito);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", pagoPorProducto.idProductor);
            return View(pagoPorProducto);
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
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", pagoPorProducto.idProductor);
            return View(pagoPorProducto);
        }

        // POST: PagosPorProductos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,cantidadProducto,numeroSemana,cheque,MyProperty,tipoProducto,garantiaLimpieza")] PagoPorProducto pagoPorProducto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pagoPorProducto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", pagoPorProducto.idProductor);
            return View(pagoPorProducto);
        }

        // GET: PagosPorProductos/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: PagosPorProductos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PagoPorProducto pagoPorProducto = db.PagosPorProductos.Find(id);
            db.MovimientosFinancieros.Remove(pagoPorProducto);
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
    }
}
