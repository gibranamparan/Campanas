using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class PrestamoYAbonoCapital:MovimientoFinanciero
    {
        //public int MesesAPagar { get; set; }
        //[DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}",
        // ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        //public DateTime fechaDePrestamo { get; set; }
        // public int rate { get; set; }
        [Display(Name = "Cheque ")]
        public string cheque { get; set; }
        [Display(Name = "Concepto ")]
        public string concepto { get; set; }

        //public double cargo { get; set; }
        [Display(Name = "Pagare")]
        public string pagare { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha a pagar ")]
        public DateTime fechaPagar { get; set; }
        [Display(Name = "Proveedor ")]
        public string proveedor { get; set; }

        //public int intereses { get; set; }
        [Display(Name = "Nota ")]
        public string nota { get; set; }



    }
}