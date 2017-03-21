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
    public class PrestamoYAbonoCapitalsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PrestamoYAbonoCapitals
        public ActionResult Index()
        {
            //var movimientosFinancieros = db.MovimientosFinancieros.Include(p => p.Productor);
            var pretamoyabonocapital = db.PrestamosYAbonosCapital.Include(p => p.Productor);
            return View(pretamoyabonocapital.ToList());
        }

        // GET: PrestamoYAbonoCapitals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoYAbonoCapital prestamoYAbonoCapital = db.PrestamosYAbonosCapital.Find(id);
            if (prestamoYAbonoCapital == null)
            {
                return HttpNotFound();
            }
            return View(prestamoYAbonoCapital);
        }

        // GET: PrestamoYAbonoCapitals/Create
        public ActionResult Create()
        {
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor");
            return View();
        }

        // POST: PrestamoYAbonoCapitals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,fechaDePrestamo,cheque,concepto,cargo,pagare,fechaPagar,proveedor,nota")] PrestamoYAbonoCapital prestamoYAbonoCapital)
        {
            double balanceAnterior = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    var movimientosAscendentes = db.MovimientosFinancieros.Where(mov => mov.idProductor == prestamoYAbonoCapital.idProductor).OrderByDescending(mov => mov.fechaMovimiento);
                    var ultimoMov = movimientosAscendentes.First();
                    balanceAnterior = ultimoMov.balance;
                }
                catch { }
                if (prestamoYAbonoCapital.concepto == "prestamo")
                {
                    prestamoYAbonoCapital.balance = balanceAnterior + prestamoYAbonoCapital.montoMovimiento;
                }
                else if(prestamoYAbonoCapital.concepto == "abono")
                {
                    prestamoYAbonoCapital.balance = balanceAnterior - prestamoYAbonoCapital.montoMovimiento;
                }
                db.MovimientosFinancieros.Add(prestamoYAbonoCapital);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", prestamoYAbonoCapital.idProductor);
            return View(prestamoYAbonoCapital);
        }

        // GET: PrestamoYAbonoCapitals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoYAbonoCapital prestamoYAbonoCapital = db.PrestamosYAbonosCapital.Find(id);
            if (prestamoYAbonoCapital == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", prestamoYAbonoCapital.idProductor);
            return View(prestamoYAbonoCapital);
        }

        // POST: PrestamoYAbonoCapitals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,fechaDePrestamo,cheque,concepto,cargo,pagare,fechaPagar,proveedor,nota")] PrestamoYAbonoCapital prestamoYAbonoCapital)
        {
            if (ModelState.IsValid)
            {
                db.Entry(prestamoYAbonoCapital).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", prestamoYAbonoCapital.idProductor);
            return View(prestamoYAbonoCapital);
        }

        // GET: PrestamoYAbonoCapitals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoYAbonoCapital prestamoYAbonoCapital = db.PrestamosYAbonosCapital.Find(id);
            if (prestamoYAbonoCapital == null)
            {
                return HttpNotFound();
            }
            return View(prestamoYAbonoCapital);
        }

        // POST: PrestamoYAbonoCapitals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PrestamoYAbonoCapital prestamoYAbonoCapital = db.PrestamosYAbonosCapital.Find(id);
            db.MovimientosFinancieros.Remove(prestamoYAbonoCapital);
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
