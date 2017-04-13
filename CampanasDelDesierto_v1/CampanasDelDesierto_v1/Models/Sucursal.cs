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
        public string nombreSucursal { get; set; }
        [Required]
        public string domicilioSucursal { get; set; }

        public virtual ICollection<Empleado> Empleados { get; set; }
        public int idEmpleado { get; set; }
    }
}