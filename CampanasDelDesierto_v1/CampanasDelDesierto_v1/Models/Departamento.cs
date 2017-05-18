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

        [Display(Name = "Domicilio de Sucursal")]
        public string domicilio { get; set; }

        //En un departamento trabajan varios empleados
        public virtual ICollection<Empleado> Empleados { get; set; }
        //Un departamento tiene muchos inventarios
        public virtual ICollection<Inventario> Inventarios { get; set; }

        public int activosDisponibles {
            get
            {
                return this.Inventarios.ToList().Sum(inv => inv.cantidadActivosDisponibles);
            }
        }

        //Un departamento tiene una sucursal
        public int idSucursal { get; set; }
        public virtual Sucursal Sucursal { get; set; }
    }
}