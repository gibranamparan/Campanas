using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class Productor
    {
        [Key]
        public int idProductor { get; set; }
        public string nombreProductor { get; set; }
        [Display(Name ="Direccion")]
        public string domicilio { get; set; }
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime fechaIntegracion { get; set; }
        public string RFC { get; set; }
        [Display(Name ="Zona")]
        public string zona { get; set; }
        [Display(Name ="Nombre del Cheque")]
        public string nombreCheque { get; set; }
        [Display(Name ="Adeudo Anterior")]
        public int? adeudoAnterior { get; set; }
        [Display(Name ="Precio de Cosecha")]
        public int? precioCosecha { get; set; }

        public ICollection<MovimientoFinanciero> MovimientosFinancieros { get; set; }
    }
}