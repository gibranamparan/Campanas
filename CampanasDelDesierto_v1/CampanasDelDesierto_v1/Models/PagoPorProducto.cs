using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    /// <summary>
    /// Representa un registro de compra de cosecha vendida por un productor
    /// </summary>
    public class PagoPorProducto : MovimientoFinanciero
    {
        [Display(Name = "Toneladas")]
        public double cantidadProducto { get; set; }

        [Display(Name = "Numero de la semana")]
        public int numeroSemana { get; set; }

        [Display(Name = "Cheque / Folio")]
        public string cheque { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Abono de anticipo ")]
        public decimal abonoAnticipo { get; set; }

        [Display(Name = "Tipo de producto ")]
        public string tipoProducto { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Pago de garantia de la limpieza")]
        public decimal garantiaLimpieza { get; set; }
        
    }
}