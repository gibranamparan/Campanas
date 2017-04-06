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

        [Display(Name = "Nombre Productor ")]
        public string nombreProductor { get; set; }

        [Display(Name ="Domicilio")]
        public string domicilio { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de integracion")]
        public DateTime fechaIntegracion { get; set; }

        [Display(Name = "RFC ")]
        public string RFC { get; set; }

        [Display(Name ="Zona")]
        public string zona { get; set; }

        [Display(Name ="Nombre del Cheque")]
        public string nombreCheque { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name ="Adeudo Anterior")]
        public decimal? adeudoAnterior { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name ="Precio de Cosecha")]
        public decimal? precioCosecha { get; set; }

        public ICollection<MovimientoFinanciero> MovimientosFinancieros { get; set; }
    }
}