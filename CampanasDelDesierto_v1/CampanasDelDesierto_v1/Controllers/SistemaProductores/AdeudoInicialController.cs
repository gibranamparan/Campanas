using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;
using CampanasDelDesierto_v1.HerramientasGenerales;
using CampanasDelDesierto_v1.Models.SistemaProductores;


namespace CampanasDelDesierto_v1.Controllers.SistemaProductores
{
    public class AdeudoInicialController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: AdeudoInicial
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var reg = db.AdeudosIniciales.Find();
            if (reg == null)
                return HttpNotFound();

            return View(reg);
        }

        // GET: AdeudoInicial
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var reg = db.AdeudosIniciales.Find(id);
            if (reg == null)
                return HttpNotFound();

            return RedirectToAction("Edit", "Productores", new { id = reg.idProductor });
        }
    }
}