using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class VentaACredito : MovimientoFinanciero
    {
        [Display(Name = "Cantidad de Material")]
        public int cantidadMaterial { get; set; }

        //Una venta a credito esta relacionada con un producto especifico
        [Display(Name = "Producto")]
        public virtual int idProducto { get; set; }
        public virtual Producto Producto { get; set; }

        public void ajustarMovimiento(Producto producto)
        {
            decimal costoProducto = producto.costo;
            decimal totalventa = costoProducto * this.cantidadMaterial;
            this.montoMovimiento = totalventa;

            base.ajustarMovimiento();
        }
    }

}