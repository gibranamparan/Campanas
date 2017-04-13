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
        [Display(Name = "Costo ")]
        public decimal costo { get; set; }
        [Display(Name = "Estado ")]
        public string estado { get; set; }

        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha ")]
        public DateTime fecha { get; set; }

        //Tipo de material que es.
        [Display(Name = "Concepto ")]
        public string concepto { get; set; }
        [Display(Name = "Pagare")]
        public string pagare { get; set; }
        [Display(Name = "Orden de Compra ")]
        public string ordenDeCompra { get; set; }


        public ICollection<VentaACredito> PrestamosMateriales { get; set; }
    }
}