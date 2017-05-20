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
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO1 + " (tons.)")]
        public double cantidadProducto1 { get; set; }

        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO2 + " (tons.)")]
        public double cantidadProducto2 { get; set; }

        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO3 + " (tons.)")]
        public double cantidadProducto3 { get; set; }

        [Display(Name = "Numero de la semana")]
        public int numeroSemana { get; set; }
    }
}