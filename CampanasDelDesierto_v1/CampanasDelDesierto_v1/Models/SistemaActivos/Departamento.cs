using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class Departamento
    {
        [Key]        
        public int departamentoID { get; set; }
        
        [Display(Name = "Nombre del departamento")]
        public string nombreDepartamento { get; set; }

        [Display(Name = "Domicilio de Departamento")]
        public string domicilio { get; set; }

        //En un departamento trabajan varios empleados
        public virtual ICollection<Empleado> Empleados { get; set; }
        //Un departamento tiene muchos inventarios
        public virtual ICollection<Activo> Activos { get; set; }

        //public int activosDisponibles()
        //{
        //    ApplicationDbContext db = new ApplicationDbContext();
        //    var productosActivos = db.ProductosActivos.Fi;
        //    return productosActivos.TakeWhile(ac => ac.prestado() == false).Count();
        //}
        //public int activosPrestados()
        //{
        //    return this.Activos.TakeWhile(ac => ac.prestado() == true).Count();
        //}

        //Un departamento tiene una sucursal
        public int idSucursal { get; set; }
        public virtual Sucursal Sucursal { get; set; }

        public virtual ICollection<AdminDepartamento> AdminsDepartamentos { get; set; }
    }
}