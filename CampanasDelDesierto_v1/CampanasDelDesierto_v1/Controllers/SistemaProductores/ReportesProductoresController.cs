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
using LiquidacionFinal = CampanasDelDesierto_v1.Models.ReportesViewModels.VMLiquidacionFinal;
using LiquidacionDeAceituna = CampanasDelDesierto_v1.Models.ReportesViewModels.VMLiquidacionDeAceituna;

namespace CampanasDelDesierto_v1.Controllers.SistemaProductores
{
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN )]
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

        [HttpGet]
        public ActionResult LiquidacionFinal(int id, int productorID)
        {
            var prod = db.Productores.Find(productorID);
            TemporadaDeCosecha temporadaConsultada = db.TemporadaDeCosechas.Find(id);
            TemporadaDeCosecha temporadaAnterior = temporadaConsultada.getTemporadaAnterior(db);
            LiquidacionFinal reporte = new LiquidacionFinal(prod, temporadaConsultada, temporadaAnterior);
            ViewBag.temporadaSeleccionada = temporadaConsultada;
            ViewBag.temporadaAnterior = temporadaAnterior;
            ViewBag.reporte = reporte;
            return View(prod);
        }

        public ActionResult LiquidacionDeAceituna(int id, int productorID)
        {
            var prod = db.Productores.Find(productorID);
            TemporadaDeCosecha temporadaConsultada = db.TemporadaDeCosechas.Find(id);
            TemporadaDeCosecha temporadaAnterior = temporadaConsultada.getTemporadaAnterior(db);
            var liquidaciones = prod.MovimientosFinancieros.Where(mov => mov.TemporadaDeCosechaID == temporadaConsultada.TemporadaDeCosechaID).ToList()
                .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.LIQUIDACION).Cast<LiquidacionSemanal>().ToList();
            LiquidacionDeAceituna reporte = new LiquidacionDeAceituna(liquidaciones, prod, temporadaConsultada, temporadaAnterior);

            //Se prepara la vista
            ViewBag.temporadaSeleccionada = temporadaConsultada;
            ViewBag.temporadaAnterior = temporadaAnterior;
            ViewBag.reporte = reporte;

            return View(prod);
        }
    }
}