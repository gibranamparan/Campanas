using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CampanasDelDesierto_v1.HerramientasGenerales;

namespace CampanasDelDesierto_v1.Models
{
    public class VentaACredito : MovimientoFinanciero
    {
        [Display(Name = "Descripcion de Concepto")]
        [DataType(DataType.MultilineText)]
        public string conceptoDeVenta { get; set; }

        [Display(Name = "Pagaré")]
        public string pagareVenta { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha a pagar")]
        public DateTime? fechaPagar { get; set; }

        [Display(Name = "Orden de Compra")]
        public string ordenCompra { get; set; }

        public virtual ICollection<CompraProducto> ComprasProductos { get; set; }

        public new string concepto {
            get {
                string res = String.Empty;
                foreach(var com in this.ComprasProductos)
                    res += com.producto.nombreProducto+",";
                res = res.TrimEnd(',');
                if (res.Length > 60) { 
                    res = res.Substring(0, 60);
                    res += "...";
                }

                if(this.tipoDeBalance == TipoDeBalance.VENTA_OLIVO)
                {
                    var compra= this.ComprasProductos.FirstOrDefault(com => com.producto.isArbolAceituna);
                    if (compra != null) {
                        res += String.Format(" ({0})", compra.cantidadMaterial);
                    }
                }

                return res;
            }
        }

        public new void ajustarMovimiento(DateTime? originalDate = null)
        {
            this.montoMovimiento *= -1;
            base.ajustarMovimiento(originalDate);
        }

        /// <summary>
        /// Indica si en una lista de productos comprados se ha hecho la venta de polen, verificando la propiedad isArbolAceituna en cada producto.
        /// </summary>
        /// <param name="comprasProductos">Lista de compras de productos</param>
        /// <returns></returns>
        internal static bool isVentaOlivo(ICollection<CompraProducto> comprasProductos)
        {
            bool hayOlivo = false;
            if (comprasProductos != null)
                foreach (var com in comprasProductos)
                    if (com.producto.isArbolAceituna)
                    {
                        hayOlivo = true;
                        break;
                    }
            return hayOlivo;
        }

        /// <summary>
        /// Indica si en una lista de productos comprados se ha hecho la venta de polen, verificando el nombre del producto.
        /// </summary>
        /// <param name="comprasProductos">Lista de compras de productos</param>
        /// <returns></returns>
        internal static bool isVentaPolen(ICollection<CompraProducto> comprasProductos)
        {
            bool hayPolen = false;
            if (comprasProductos != null)
                foreach (var com in comprasProductos)
                    if (com.producto.nombreProducto.Contains("polen",StringComparison.OrdinalIgnoreCase))
                    {
                        hayPolen = true;
                        break;
                    }
            return hayPolen;
        }
    }

}