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
    [Authorize(Roles = "Admin")]
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
        public ActionResult Create(int? id = 0, int? anioCosecha=0)
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

            var mov = prepararVistaCrear(productor, anioCosecha.Value);

            mov.introducirMovimientoEnPeriodo(anioCosecha);

            return View(mov);
        }

        private PagoPorProducto prepararVistaCrear(Productor productor, int? anioCosecha)
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
            "cantidadProducto,numeroSemana,cheque,abonoAnticipo,tipoProducto,garantiaLimpieza")]
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
                        new { id = pagoPorProducto.idProductor, anioCosecha = pagoPorProducto.anioCosecha });
                }
            }

            var mov = prepararVistaCrear(db.Productores.Find(pagoPorProducto.idProductor), pagoPorProducto.fechaMovimiento.Year);

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
            "cantidadProducto,numeroSemana,cheque,abonoAnticipo,tipoProducto,garantiaLimpieza")]
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
                    return RedirectToAction("Details", "Productores", new { id = pagoPorProducto.idProductor, anioCosecha = pagoPorProducto.anioCosecha });
                }
            }

            ViewBag.productor = pagoPorProducto.Productor;

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
