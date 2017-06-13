using CampanasDelDesierto_v1.HerramientasGenerales;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public double cantidadProducto1 { get; set; }

        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO2 + " (tons.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public double cantidadProducto2 { get; set; }

        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO3 + " (tons.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public double cantidadProducto3 { get; set; }

        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO1 + " (USD)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal pagoProducto1 { get; set; }

        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO2 + " (USD)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal pagoProducto2 { get; set; }

        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO3 + " (USD)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal pagoProducto3 { get; set; }

        [DisplayName("Semana")]
        [Range(1,53)] //Rango de semanas en 1 año
        [Required]
        public int semana { get; set; }

        public virtual ICollection<RecepcionDeProducto> Recepciones { get; set; }

        [ForeignKey("liquidacion")]
        public int? liquidacionID { get; set; }
        public virtual LiquidacionSemanal liquidacion { get; set; }

        public int eliminarAsociacionConRecepciones(ApplicationDbContext db)
        {
            int numRegs = 0;
            if (this.Recepciones!=null && this.Recepciones.Count() > 0)
            {
                foreach(var rec in Recepciones.ToList())
                {
                    rec.movimientoID = null;
                    db.Entry(rec).State = System.Data.Entity.EntityState.Modified;
                }
                numRegs = db.SaveChanges();
            }
            return numRegs;
        }

    }
}