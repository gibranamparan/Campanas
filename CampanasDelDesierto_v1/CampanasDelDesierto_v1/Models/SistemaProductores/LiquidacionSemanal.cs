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

        [DisplayName("Semana")]
        [Range(1, 53)] //Rango de semanas en 1 año
        [Required]
        public int semana { get; set; }

        public new string concepto
        {
            get
            {
                return String.Format($"CHEQUE: {this.cheque}");
            }
        }

        /*RENTECIONES*/
        [Display(Name = "Abono Anticipos")]
        [ForeignKey("abonoAnticipo")]
        public int? abonoAnticipoID { get; set; }
        public virtual PrestamoYAbonoCapital abonoAnticipo { get; set; }
        
        [Display(Name = "Abono Arboles")]
        [ForeignKey("abonoArboles")]
        public int? abonoArbolesID { get; set; }
        public virtual PrestamoYAbonoCapital abonoArboles { get; set; }

        //Lista de retenciones asociadas a la liquidacion
        public virtual ICollection<Retencion> retenciones { get; set; }

        //Lista de ingresos de cosecha liquidados en esta instancia
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
            if (tipo == Retencion.TipoRetencion.ABONO_ANTICIPO && this.abonoAnticipo!=null)
            {
                monto = this.abonoAnticipo.montoMovimiento;
            }else if(this.retenciones.FirstOrDefault(mov => mov.tipoDeDeduccion == tipo)!=null)
            {
                monto = -this.retenciones
                .FirstOrDefault(mov => mov.tipoDeDeduccion == tipo).montoMovimiento;
            }

            return monto;
        }

        public class VMRetenciones
        {
            /// <summary>
            /// Retención de garantía por sanidad.
            /// </summary>
            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [Display(Name = Retencion.NombreRetencion.SANIDAD)]
            public decimal garantiaLimpieza { get; set; }

            //Retención ejidal
            /// <summary>
            /// Retencion de aportación de 2% ejidal
            /// </summary>
            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [Display(Name = Retencion.NombreRetencion.EJIDAL)]
            public decimal retencionEjidal { get; set; }

            /// <summary>
            /// Retención para abonar a balance de anticipos
            /// </summary>
            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [Display(Name = Retencion.NombreRetencion.ABONO_ANTICIPO)]
            public decimal abonoAnticipos { get; set; }
            
            /// <summary>
            /// Retencion de abono a balance de venta de arboles de olivo
            /// </summary>
            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [Display(Name = Retencion.NombreRetencion.ABONO_ARBOLES)]
            public decimal abonoArboles { get; set; }

            /// <summary>
            /// Otro tipo de retenciones sumarizadas
            /// </summary>
            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [Display(Name = Retencion.NombreRetencion.OTRO)]
            public decimal retencionOtro { get; set; }

            public decimal total
            {
                get
                {
                    return this.garantiaLimpieza + this.abonoAnticipos + this.retencionEjidal + this.retencionOtro;
                }
            }
        }

        public class VMRetencionReporteSemanal
        {

            [DisplayFormat(DataFormatString = "{0:0.00}",
            ApplyFormatInEditMode = true)]
            [Display(Name = "Retención de la Semana")]
            public decimal garantiaSemana { get; set; }

            [DisplayFormat(DataFormatString = "{0:0.00}",
            ApplyFormatInEditMode = true)]
            [Display(Name = "Retención Actual")]
            public decimal garantiaActual { get; set; }

            [DisplayFormat(DataFormatString = "{0:0.00}",
            ApplyFormatInEditMode = true)]
            [Display(Name = "Retención Acumulada")]
            public decimal garantiaAcumulada { get; set; }

            public VMRetencionReporteSemanal(List<Retencion> retencionesDeSanidad, LiquidacionSemanal liquidacionReportada, Retencion.TipoRetencion tipo)
            {
                //Se toman todas las retenciones de sanidad anteriores al reporte actual
                retencionesDeSanidad = retencionesDeSanidad.Where(mov => mov.tipoDeDeduccion == tipo
                && mov.fechaMovimiento.Date <= liquidacionReportada.fechaMovimiento.Date && mov.TemporadaDeCosechaID == liquidacionReportada.TemporadaDeCosechaID)
                .OrderBy(mov => mov.fechaMovimiento).ToList();

                //Retenciones de sanidad acumuladas hasta la fecha
                this.garantiaAcumulada = Math.Abs(retencionesDeSanidad.Sum(mov => mov.montoMovimiento));
                this.garantiaActual = Math.Abs(this.garantiaAcumulada);

                Retencion ultimaRetencion = new Retencion();

                //Se esta editando un reporte de liquidacion semanal
                if (liquidacionReportada.idMovimiento != 0 && liquidacionReportada.retenciones!=null && liquidacionReportada.retenciones.Count() > 0)
                {
                    //Se busca el tipo de retencion a la que se le esta preparando el reporte
                    Retencion retActual = liquidacionReportada.retenciones.FirstOrDefault(mov => mov.tipoDeDeduccion == tipo);
                    if (retActual != null) { 
                        this.garantiaSemana = Math.Abs(retActual.montoMovimiento);
                        this.garantiaAcumulada -= this.garantiaSemana;
                    }
                }
            }
        }
    }
}