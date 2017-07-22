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
        /*CANTIDAD MANZANITA*/
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO1 + " (tons.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public double cantidadProducto1 { get; set; }

        /*CANTIDAD OBLISSA*/
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO2 + " (tons.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public double cantidadProducto2 { get; set; }

        /*CANTIDAD MISSION*/
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO3 + " (tons.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public double cantidadProducto3 { get; set; }

        /*CANTIDAD MANZANITA ORG*/
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO4 + " (tons.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public double cantidadProducto4 { get; set; }

        /*CANTIDAD OBLIZA ORG*/
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO5 + " (tons.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public double cantidadProducto5 { get; set; }

        /*CANTIDAD MISSION ORG*/
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO6 + " (tons.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public double cantidadProducto6 { get; set; }

        /*PAGO MANZANITA*/
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO1 + " (USD)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal pagoProducto1 { get; set; }

        /*PAGO OBLISSA*/
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO2 + " (USD)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal pagoProducto2 { get; set; }

        /*PAGO MISSION*/
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO3 + " (USD)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal pagoProducto3 { get; set; }

        /*PAGO MANZANITA ORG*/
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO4 + " (USD)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal pagoProducto4 { get; set; }

        /*PAGO OBLIZA ORG*/
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO5 + " (USD)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal pagoProducto5 { get; set; }

        /*PAGO MISSION ORG*/
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO6 + " (USD)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal pagoProducto6 { get; set; }

        [DisplayName("Semana")]
        [Range(1,53)] //Rango de semanas en 1 año
        [Required]
        public int semana { get; set; }

        public bool yaLiquidado { get {
                return this.liquidacionDeCosechaID != null && this.liquidacionDeCosechaID != 0;
            } }

        public new string concepto { get {
                string res = "SEMANA: " + this.semana;
                res+= this.liquidacionDeCosecha == null ? "" 
                    : String.Format($", CHEQUE: {this.liquidacionDeCosecha.cheque}");
                return res;
            } }

        public virtual ICollection<RecepcionDeProducto> Recepciones { get; set; }

        [ForeignKey("liquidacionDeCosecha")]
        public int? liquidacionDeCosechaID { get; set; }
        public virtual LiquidacionSemanal liquidacionDeCosecha { get; set; }

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