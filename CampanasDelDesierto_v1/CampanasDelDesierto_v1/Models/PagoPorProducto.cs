using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class PagoPorProducto : MovimientoFinanciero
    {
        [Display(Name = "Cantidad de productos ")]
        public int cantidadProducto { get; set; }
        [Display(Name = "Numero de la semana")]
        public int numeroSemana { get; set; }
        [Display(Name = "Cheque ")]
        public string cheque { get; set; }
        [Display(Name = "Abono de anticipo ")]
        public double abonoAnticipo { get; set; }
        [Display(Name = "Tipo de producto ")]
        public string tipoProducto { get; set; }
        [Display(Name = "Pago de garantia de la limpieza")]
        public double garantiaLimpieza { get; set; }
    }
}