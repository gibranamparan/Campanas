using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class Sucursal
    {
        [Key]
        public int idSucursal { get; set; }

        [Required]
        [Display(Name = "Nombre Sucursal")]
        public string nombreSucursal { get; set; }

        [Required]
        [Display(Name = "Domicilio de Sucursal")]
        public string domicilioSucursal { get; set; }

        //En una sucursal trabajan varios empleados
        public virtual ICollection<Empleado> Empleados { get; set; }
    }
}