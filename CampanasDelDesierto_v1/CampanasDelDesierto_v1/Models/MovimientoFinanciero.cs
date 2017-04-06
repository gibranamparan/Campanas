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


        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Monto del movimiento ")]
        public double montoMovimiento { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Balance ")]
        public double balance { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha del movimiento ")]
        public DateTime fechaMovimiento { get; set; }

        public virtual Productor Productor { get; set; }
        public virtual int idProductor { get; set; }


    }
}