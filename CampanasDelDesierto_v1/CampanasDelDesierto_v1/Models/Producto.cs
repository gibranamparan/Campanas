using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class Producto
    {
        [Key]
        public int idProducto { get; set; }

        [Display(Name = "Nombre ")]
        public string nombreProducto { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Costo ")]
        public decimal costo { get; set; }

        [Display(Name = "Estado ")]
        public string estado { get; set; }
        
        //Tipo de material que es.
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        public ICollection<VentaACredito> ventas { get; set; }
    }
}