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
    public class LiquidacionSemanal:MovimientoFinanciero
    {
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Cheque / Folio")]
        public string cheque { get; set; }

        [Display(Name = "Periodo")]
        public TimePeriod semanaLiquidada { get; set; }

        [Display(Name = "Precio del dólar")]
        [DisplayFormat(DataFormatString = "{0:C4}",
        ApplyFormatInEditMode = true)]
        [DecimalPrecision(18, 4)]
        public decimal precioDelDolarEnLiquidacion { get; set; }

        /*RENTECIONES*/
        [Display(Name = "Abono a Anticipos")]
        [ForeignKey("abonoAnticipo")]
        public int? abonoAnticipoID { get; set; }
        public virtual PrestamoYAbonoCapital abonoAnticipo { get; set; }

        [DisplayName("Semana")]
        [Range(1, 53)] //Rango de semanas en 1 año
        [Required]
        public int semana { get; set; }

        public new string concepto { get {
                return String.Format($"CHEQUE: {this.cheque}");
            } }

        //Lista de retenciones asociadas a la liquidacion
        public virtual ICollection<Retencion> retenciones { get; set; }

        public virtual ICollection<PagoPorProducto> ingresosDeCosecha { get; set; }

        public LiquidacionSemanal()
        {
            this.semanaLiquidada = new TimePeriod();
            this.semanaLiquidada.endDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);
            this.semanaLiquidada.startDate = this.semanaLiquidada.endDate.AddDays(-8).AddMilliseconds(1);
        }

        public new void ajustarMovimiento()
        {
            this.montoMovimiento *= -1;
            base.ajustarMovimiento();
        }

        public decimal getMontoRetencion(Retencion.TipoRetencion tipo)
        {
            decimal monto = 0;
            if (tipo == Retencion.TipoRetencion.ABONO && this.abonoAnticipo!=null)
            {
                monto = -this.abonoAnticipo.montoMovimiento;
            }else if(this.retenciones.FirstOrDefault(mov => mov.tipoDeDeduccion == tipo)!=null)
            {
                monto = -this.retenciones
                .FirstOrDefault(mov => mov.tipoDeDeduccion == tipo).montoMovimiento;
            }

            return monto;
        }

        public class VMRetenciones
        {
            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [Display(Name = "Garantia de Sanidad Vegetal ")]
            public decimal garantiaLimpieza { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [Display(Name = "2% I.S.R Ejidal")]
            public decimal retencionEjidal { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [Display(Name = "Abono a Anticipos")]
            public decimal abonoAnticipos { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [Display(Name = "Otra Retención")]
            public decimal retencionOtro { get; set; }

            public decimal total
            {
                get
                {
                    return this.garantiaLimpieza + this.abonoAnticipos + this.retencionEjidal + this.retencionOtro;
                }
            }
        }
    }
}