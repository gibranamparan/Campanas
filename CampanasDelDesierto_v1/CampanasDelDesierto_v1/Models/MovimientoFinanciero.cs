using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class MovimientoFinanciero
    {
        [Key]
        public int idMovimiento { get; set; }
        public int montoMovimiento { get; set; }
        [DisplayFormat(DataFormatString = "{0: yyyy/MM/dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime fechaMovimiento { get; set; }

        public virtual Productor Productor { get; set; }
        public virtual int idProductor { get; set; }


    }
}