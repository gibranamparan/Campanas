using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class Activo
    {
        [Key]
        public int idActivos { get; set; }
        [Display(Name ="Nombre ")]
        public string nombreActivo { get; set; }
        public decimal costo { get; set; }
        public string estado { get; set; }
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime fecha { get; set; }
        //Tipo de material que es.
        public string concepto { get; set; }
        public string pagare { get; set; }
        public string ordenDeCompra { get; set; }


        public ICollection<VentaACredito> PrestamosMateriales { get; set; }
    }
}