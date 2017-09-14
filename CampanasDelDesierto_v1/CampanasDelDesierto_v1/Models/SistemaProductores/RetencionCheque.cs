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

        [Required]
        [ForeignKey("retencion")]
        public int retencionID { get; set; }
        public virtual Retencion retencion { get; set; }

        [DisplayName("Monto")]
        [DisplayFormat(DataFormatString = "{0:C}", 
            ApplyFormatInEditMode = true)]
        public decimal monto { get; set; }
    }
}