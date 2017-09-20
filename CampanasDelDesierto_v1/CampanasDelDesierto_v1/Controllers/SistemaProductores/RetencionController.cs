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
        public const string BIND_FIELDS_CHEQUE = "chequeID,numCheque,fecha,productorID,temporadaID,tipoDeDeduccion,monto";
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Retencion
        public ActionResult cheques(Retencion.TipoRetencion tipoRetencion = Retencion.TipoRetencion.NINGUNO, int productorID = 0,int temporadaID =0)
        {
            if (productorID==0 || temporadaID==0 || tipoRetencion == Retencion.TipoRetencion.NINGUNO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Productor productor= db.Productores.Find(productorID);
            TemporadaDeCosecha temporada = db.TemporadaDeCosechas.Find(temporadaID);
            if (productor == null || temporada==null)
                return HttpNotFound();
            Productor.VMLiberacionRetencionesStatus vmStatus = new Productor.VMLiberacionRetencionesStatus(productor, temporada, tipoRetencion);

            return View(vmStatus);
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

            return RedirectToAction("cheques", new { productorID = cheque.productorID, temporadaID = cheque.temporadaID,
                tipoRetencion = cheque.tipoDeDeduccion });
        }

        // POST: Retencion/crear
        public ActionResult cancelar(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            int numReg = 0;

            var cheque = db.ChequesDeRetenciones.Find(id);
            int temporadaID = cheque.temporadaID, productorID = cheque.productorID;
            Retencion.TipoRetencion tipoRetencion = cheque.tipoDeDeduccion;

            db.ChequesDeRetenciones.Remove(cheque);
            numReg = db.SaveChanges();

            return RedirectToAction("cheques", new {productorID = productorID,temporadaID = temporadaID,
                tipoRetencion = tipoRetencion});
        }

        public ActionResult PrintCheque(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RetencionCheque retencion = db.ChequesDeRetenciones.Find(id);
            if (retencion == null)
            {
                return HttpNotFound();
            }
            LiquidacionSemanal.VMDatosDeCheque datosCheque = new LiquidacionSemanal.VMDatosDeCheque(retencion);
            return View("../LiquidacionSemanal/Cheque", datosCheque);
        }
    }
}