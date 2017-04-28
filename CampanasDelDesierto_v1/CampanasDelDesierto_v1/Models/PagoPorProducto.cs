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

        [Display(Name = "Tipo de producto ")]
        public string tipoProducto { get; set; }

    }
}