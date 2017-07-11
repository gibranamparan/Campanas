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
    }

}