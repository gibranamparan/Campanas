using CampanasDelDesierto_v1.Models;
using CampanasDelDesierto_v1.Models.SistemaProductores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CampanasDelDesierto_v1.Controllers.SistemaProductores
{
    public class RetencionController : Controller
    {
        public const string BIND_FIELDS_CHEQUE = "chequeID,numCheque,fecha,retencionID,monto";
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Retencion
        public ActionResult cheques(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Retencion rentecion = db.Retenciones.Find(id);
            if (rentecion == null)
                return HttpNotFound();

            return View(rentecion);
        }

        // POST: Retencion/crear
        public ActionResult crear([Bind(Include = BIND_FIELDS_CHEQUE)]RetencionCheque cheque)
        {
            int numReg = 0;
            if (ModelState.IsValid)
            {
                db.ChequesDeRetenciones.Add(cheque);
                numReg = db.SaveChanges();
            }

            return RedirectToAction("cheques", new { id = cheque.retencionID });
        }

        // POST: Retencion/crear
        public ActionResult cancelar(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            int numReg = 0;

            var cheque = db.ChequesDeRetenciones.Find(id);
            int chequeID = cheque.chequeID;

            db.ChequesDeRetenciones.Remove(cheque);
            numReg = db.SaveChanges();

            return RedirectToAction("cheques", new { id = cheque.retencionID });
        }
    }
}