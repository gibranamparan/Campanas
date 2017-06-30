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
using AdeudoRecuperacionReg = CampanasDelDesierto_v1.Models.ReportesViewModels.VMAdeudosRecuperacionReg;

namespace CampanasDelDesierto_v1.Controllers.SistemaProductores
{
    public class ReportesProductoresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReportesProductores
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AdeudosRecuperacion(int id)
        {
            var productores = db.Productores.ToList();
            List<AdeudoRecuperacionReg> reporte = new List<AdeudoRecuperacionReg>();
            productores.ForEach(prod => reporte.Add(new AdeudoRecuperacionReg(prod, id)));
            ViewBag.temporada = db.TemporadaDeCosechas.Find(id);
            return View(reporte);
        }
    }
}