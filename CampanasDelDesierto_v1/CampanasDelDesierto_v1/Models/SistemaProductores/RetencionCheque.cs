using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models.SistemaProductores
{
    public class RetencionCheque
    {
        [Key]
        public int chequeID { get; set; }

        [DisplayName("Num. Cheque")]
        [Required]
        public string numCheque { get; set; }

        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
            ApplyFormatInEditMode = true)]
        public DateTime fecha { get; set; }

        /// <summary>
        /// La liberacion de cheque para suma de retenciones esta relacionada con un productor especifico.
        /// </summary>
        [Required]
        [ForeignKey("productor")]
        [DisplayName("Productor")]
        public int productorID { get; set; }
        public virtual Productor productor { get; set; }

        /// <summary>
        /// La liberacion de cheque para suma de retenciones esta relacionada con una temporada especifica.
        /// </summary>
        [Required]
        [ForeignKey("temporada")]
        [DisplayName("Año de Cosecha")]
        public int temporadaID { get; set; }
        public virtual TemporadaDeCosecha temporada { get; set; }

        [DisplayName("Monto")]
        [DisplayFormat(DataFormatString = "{0:C}", 
            ApplyFormatInEditMode = true)]
        public decimal monto { get; set; }

        [DisplayName("Deduccion")]
        public Retencion.TipoRetencion tipoDeDeduccion { get; set; }

        public string nombreTipoDeduccion
        {
            get { return Retencion.getNombreTipoRetencion(this.tipoDeDeduccion); }
        }
    }
}