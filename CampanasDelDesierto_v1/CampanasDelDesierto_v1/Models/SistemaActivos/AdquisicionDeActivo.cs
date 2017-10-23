using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class AdquisicionDeActivo
    {
        [Key]
        public int AdquirirActivosID { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Cantidad de Activos")]
        public int cantidadActivo { get; set; }

        //Referencias 
        [Display(Name = "Producto Activo")]       
        public int? ProductoActivoID { get; set; }
        public virtual ProductoActivo ProductoActivo { get; set; }

        [Display(Name = "Prestamo")]
        [ForeignKey("prestamo")]
        public int idPrestamoActivo { get; set; }
        public virtual PrestamoActivo prestamo { get; set; }
    }
}