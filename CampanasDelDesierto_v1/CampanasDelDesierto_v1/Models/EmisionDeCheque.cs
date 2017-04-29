using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class EmisionDeCheque:MovimientoFinanciero
    {
        [Display(Name = "Cheque / Folio")]
        public string cheque { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Abono de anticipo (USD)")]
        public decimal abonoAnticipo { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Pago de garantia de la limpieza (USD)")]
        public decimal garantiaLimpieza { get; set; }
    }
}