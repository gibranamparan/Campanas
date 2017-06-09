using CampanasDelDesierto_v1.HerramientasGenerales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class LiquidacionSemanal:MovimientoFinanciero
    {
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
        [Display(Name = "Garantia de Sanidad Vegetal ")]
        [ForeignKey("garantiaLimpieza")]
        public int? garantiaLimpiezaID { get; set; }
        public PrestamoYAbonoCapital garantiaLimpieza { get; set; }

        [Display(Name = "2% I.S.R Ejidal")]
        [ForeignKey("rentecionEjidal")]
        public int? rentecionEjidalID { get; set; }
        public PrestamoYAbonoCapital rentecionEjidal { get; set; }

        [Display(Name = "Abono a Anticipos")]
        [ForeignKey("abonoAnticipo")]
        public int? abonoAnticipoID { get; set; }
        public PrestamoYAbonoCapital abonoAnticipo { get; set; }

        [Display(Name = "Otra Retención")]
        [ForeignKey("retencionOtro")]
        public int? retencionOtroID { get; set; }
        public PrestamoYAbonoCapital retencionOtro { get; set; }

        public LiquidacionSemanal()
        {
            this.semanaLiquidada = new TimePeriod();
            this.semanaLiquidada.endDate = DateTime.Now;
            this.semanaLiquidada.startDate = this.semanaLiquidada.endDate.AddDays(-7);
        }

        public new void ajustarMovimiento()
        {
            this.montoMovimiento *= -1;
            base.ajustarMovimiento();
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
        }
    }
}