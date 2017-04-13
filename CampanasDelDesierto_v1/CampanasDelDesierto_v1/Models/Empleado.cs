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
        public string nombre { get; set; }
        [Required]
        public string apellidoPaterno { get; set; }
        [Required]
        public string apellidoMaterno { get; set; }


        public virtual ICollection<PrestamoActivo> PrestamosActivos { get; set; }
        public int idPrestamo { get; set; }
        public virtual int idSucursal { get; set; }

    }
}