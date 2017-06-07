using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class VentaACredito : MovimientoFinanciero
    {
        [Display(Name = "Descripcion de Concepto")]
        [DataType(DataType.MultilineText)]
        public string conceptoDeVenta { get; set; }

        [Display(Name = "Pagaré")]
        public string pagareVenta { get; set; }

        public virtual ICollection<CompraProducto> ComprasProductos { get; set; }

        public new void ajustarMovimiento()
        {
            this.montoMovimiento *= -1;
            base.ajustarMovimiento();
        }
    }

}