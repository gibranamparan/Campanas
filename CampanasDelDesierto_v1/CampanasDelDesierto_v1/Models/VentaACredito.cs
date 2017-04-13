using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class VentaACredito:MovimientoFinanciero
    {
        [Display(Name = "Cantidad del Material ")]
        public int cantidadMaterial { get; set; }
        [Display(Name = "ID activos ")]
        public virtual int idProducto { get; set; }

        public virtual Producto Producto { get; set; }
    }
}