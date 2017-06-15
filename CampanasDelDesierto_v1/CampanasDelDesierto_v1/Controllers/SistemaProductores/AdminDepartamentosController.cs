using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CampanasDelDesierto_v1.Controllers.SistemaProductores
{
    public class AdminDepartamentosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AdminDepartamentos
        public ActionResult Index()
        {
            var AdminDepartamento = db.AdminsDepartamentos.ToList();
            var admins = db.Users.ToList().Where((adm)=>AdminDepartamento.Find(ad => ad.Id==adm.Id)==null).ToList();
            ViewBag.admins = admins;
            return View(AdminDepartamento);
        }

        public ActionResult Details(string id, bool errorGuest = false)
        {
            AdminDepartamento admins = null;
            //If owner, show only his details
            if (User.IsInRole(ApplicationUser.RoleNames.DEPARTAMENTO))
                admins = db.AdminsDepartamentos.Find(User.Identity.GetUserId());
            else if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //If admin, get the requested owner
            else if (User.IsInRole(ApplicationUser.RoleNames.ADMIN))
                admins = db.AdminsDepartamentos.Find(id);

            if (admins == null)
                return HttpNotFound();
            var user = db.AdminsDepartamentos.Find(id);
            ViewBag.Departamentos = db.Departamentos.Where(dep=>dep.departamentoID==user.departamentoID);
               
            ViewBag.errorGuest = errorGuest;

            return View(admins);
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
