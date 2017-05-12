﻿using System;
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
    [Authorize(Roles ="Admin")]
    public class ProductoresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Productores
        public ActionResult Index()
        {
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
            
            ViewBag.temporadas = db.TemporadaDeCosechas.ToList();
            ViewBag.temporada = tem.TemporadaDeCosechaID;

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
        public ActionResult Create([Bind(Include = "idProductor,nombreProductor,domicilio,fechaIntegracion,RFC,zona,"+
            "nombreCheque,adeudoAnterior")]
            Productor productor)
        {
            if (ModelState.IsValid)
            {
                //Si existe adeudo anterior, se introduce como movimiento de anticipo con
                //dicho concepto
                if (productor.adeudoAnterior > 0)
                {
                    String errorMsg = "";
                    //Inicializa servicio de doalres
                    HerramientasGenerales.BaxicoWebService bws = new HerramientasGenerales.BaxicoWebService();
                    //Lista de movimientos para nuevo productor
                    productor.MovimientosFinancieros = new List<MovimientoFinanciero>();
                    //Se determinar la ultima temporada
                    TemporadaDeCosecha tc = TemporadaDeCosecha.getUltimaTemporada();
                    //Se crea el nuevo movimiento que representa la deuda anterior
                    PrestamoYAbonoCapital deuda = new PrestamoYAbonoCapital();

                    //Se rellenan los atributos del nuevo movimiento
                    deuda.introducirMovimientoEnPeriodo(tc);
                    deuda.temporadaDeCosecha = null; //Evitando error de multiple tracking
                    deuda.montoMovimiento = -productor.adeudoAnterior.Value;
                    deuda.balance = deuda.montoMovimiento;
                    deuda.concepto = PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO;
                    deuda.divisa = PrestamoYAbonoCapital.Divisas.USD;
                    deuda.precioDelDolar = bws.getCambioDolar(ref errorMsg);
                    deuda.proveedor = "DEUDA ANTERIOR";
                    deuda.fechaPagar = new DateTime(deuda.fechaMovimiento.Year, 8, 15);
                    if (deuda.fechaMovimiento > deuda.fechaPagar)
                        deuda.fechaPagar = deuda.fechaPagar.Value.AddYears(1);

                    //Ajuste de movimiento para entrar dentro de la lista de balances
                    deuda.ajustarMovimiento();
                    //Se agrega el movimiento al nuevo productor
                    productor.MovimientosFinancieros.Add(deuda);
                }

                productor.RFC = productor.RFC.ToUpper();
                productor.nombreCheque = productor.nombreCheque.ToUpper();
                productor.nombreProductor = productor.nombreProductor.ToUpper();

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
            return View(productor);
        }

        // POST: Productores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idProductor,nombreProductor,domicilio,fechaIntegracion,RFC,zona,"+
            "nombreCheque,adeudoAnterior,precioProducto1,precioProducto2,precioProducto3,precioProductoOtro")]
                Productor productor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productor);
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
    }
}
