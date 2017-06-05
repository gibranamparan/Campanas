using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class CompraProducto
    {
        [Key]
        public int compraID { get; set; }

        [Display(Name = "Producto")]
        public virtual int? idProducto { get; set; }
        public virtual Producto Producto { get; set; }

        [Display(Name = "Venta")]
        public virtual int idMovimiento { get; set; }
        public virtual VentaACredito venta { get; set; }
    }
}