using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class PagoPorProducto:MovimientoFinanciero
    {
        [Display(Name = "Cantidad de productos ")]
        public double cantidadProducto { get; set; }
        [Display(Name = "Numero de la semana")]
        public int numeroSemana { get; set; }
        [Display(Name = "Cheque ")]
        public string cheque { get; set; }
        [Display(Name = "Abono de anticipo ")]
        public int? abonoAnticipo { get; set; }
        [Display(Name = "Tipo de produto ")]
        public string tipoProducto { get; set; }
        [Display(Name = "Garantia de la limpieza")]
        public int? garantiaLimpieza { get; set; }
        //public int porcentajeEjidal { get; set; }
    }
}