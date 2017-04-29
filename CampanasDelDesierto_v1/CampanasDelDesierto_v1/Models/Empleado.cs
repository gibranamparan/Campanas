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

        
        [Display(Name = "Nombre del Empleado")]
        public string nombre { get; set; }

        
        [Display(Name = "Apellido Paterno")]
        public string apellidoPaterno { get; set; }

       
        [Display(Name = "Apellido Materno")]
        public string apellidoMaterno { get; set; }

        
        
        public virtual Departamento Departamento { get; set; }
        public int departamentoID { get; set; }
        public virtual ICollection<PrestamoActivo> PrestamosActivos { get; set; }
    }
}