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
        public string cheque { get; set; }
        public string concepto { get; set; }
        //public double cargo { get; set; }
        public string pagare { get; set; }
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime fechaPagar { get; set; }
        public string proveedor { get; set; }
        //public int intereses { get; set; }
        public string nota { get; set; }



    }
}