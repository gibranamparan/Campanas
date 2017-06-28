using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class CompraProducto
    {
        [Key]
        public int compraID { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Cantidad de Material")]
        public int cantidadMaterial { get; set; }

        [Display(Name = "Producto")]
        [ForeignKey("producto")]
        public int? idProducto { get; set; }
        public virtual Producto producto { get; set; }

        [Display(Name = "Venta")]
        [ForeignKey("venta")]
        public int idMovimiento { get; set; }
        public virtual VentaACredito venta { get; set; }
    }
}