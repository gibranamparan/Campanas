using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class Empleado
    {
        [Key]
        public int idEmpleado { get; set; }

        [Required]
        [Display(Name = "Nombre del Empleado")]
        public string nombre { get; set; }

        [Required]
        [Display(Name = "Apellido Paterno")]
        public string apellidoPaterno { get; set; }

        [Required]
        [Display(Name = "Apellido Materno")]
        public string apellidoMaterno { get; set; }

        [Display(Name = "Sucursal")]
        public int idSucursal { get; set; }
        public virtual Sucursal Sucursal { get; set; }

        public virtual ICollection<PrestamoActivo> PrestamosActivos { get; set; }
    }
}