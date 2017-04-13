using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class PrestamoActivo
    {
        [Key]        
        public int idPrestamoActivo { get; set; }
        [Display(Name = "Fecha de prestamo")]
        [Required]
        public DateTime fechaPrestamoActivo { get; set; }
        [Display(Name = "Fecha de entrega")]
        [Required]
        public DateTime fechaEntregaActivo { get; set; }
        [Display(Name = "Fecha de devolucion")]        
        public DateTime? fechaDevolucion { get; set; }

        public virtual int idEmpleado { get; set; }
        public  virtual int idActivo { get; set; }
    }
}