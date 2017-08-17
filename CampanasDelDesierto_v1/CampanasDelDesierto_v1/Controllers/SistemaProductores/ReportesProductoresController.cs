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
using AdeudosRecuperacionDetallado = CampanasDelDesierto_v1.Models.ReportesViewModels.VMAdeudosRecuperacionDetallado;

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
        public ActionResult AdeudosRecuperacion(int? id)
        {
            if (id==null)
                return HttpNotFound();
            else
            {
                var productores = db.Productores.ToList();
                TemporadaDeCosecha temporadaConsultada = db.TemporadaDeCosechas.Find(id);
                TemporadaDeCosecha temporadaAnterior = temporadaConsultada.getTemporadaAnterior(db);
                List<AdeudoRecuperacionReg> reporte = new List<AdeudoRecuperacionReg>();
                productores.ForEach(prod => reporte.Add(new AdeudoRecuperacionReg(prod, temporadaConsultada, temporadaAnterior)));
                ViewBag.temporada = db.TemporadaDeCosechas.Find(id);
                return View(reporte);
            }
            
        }

        [HttpGet]
        public ActionResult AdeudosRecuperacionDetallado(int? id)
        {
            if (id == null)
                return HttpNotFound();
            else
            {
                var productores = db.Productores.ToList();
                TemporadaDeCosecha temporadaConsultada = db.TemporadaDeCosechas.Find(id);
                TemporadaDeCosecha temporadaAnterior = temporadaConsultada.getTemporadaAnterior(db);
                List<AdeudosRecuperacionDetallado> reporte = new List<AdeudosRecuperacionDetallado>();
                productores.ForEach(prod => reporte.Add(new AdeudosRecuperacionDetallado(prod, temporadaConsultada, temporadaAnterior)));
                ViewBag.temporada = temporadaConsultada;
                ViewBag.temporadaAnterior = temporadaAnterior;
                return View(reporte);
            }
        }
    }
}