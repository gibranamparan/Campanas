using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class PagoPorProducto:MovimientoFinanciero
    {        
        public double cantidadProducto { get; set; }
        public int numeroSemana { get; set; }
        public string cheque { get; set; }
        public int? abonoAnticipo { get; set; }
        public string tipoProducto { get; set; }
        public int? garantiaLimpieza { get; set; }
        //public int porcentajeEjidal { get; set; }




    }
}