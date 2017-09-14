using CampanasDelDesierto_v1.Models.SistemaProductores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class Retencion : MovimientoFinanciero
    {
        [Required]
        [ForeignKey("liquidacionSemanal")]
        public int liquidacionSemanalID { get; set; }
        public virtual LiquidacionSemanal liquidacionSemanal { get; set; }
        public TipoRetencion tipoDeDeduccion { get; set; }

        public Retencion() { }
        public Retencion(LiquidacionSemanal emisionDeCheque, decimal monto, TipoRetencion td)
        {
            this.montoMovimiento = monto;
            this.fechaMovimiento = emisionDeCheque.fechaMovimiento;
            this.liquidacionSemanalID = emisionDeCheque.idMovimiento;
            this.TemporadaDeCosechaID = emisionDeCheque.TemporadaDeCosechaID;
            this.idProductor = emisionDeCheque.idProductor;
            this.tipoDeDeduccion = td;
            this.ajustarMovimiento();
        }

        /// <summary>
        /// Cheques generados para liberar el pago de rentecion.
        /// </summary>
        [DisplayName("Cheques de Pago de Retención")]
        public virtual ICollection<RetencionCheque> cheques { get; set; }

        [DisplayName("Monto pagado")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal montoPagado
        {
            get
            {
                decimal res = 0;
                if (this.cheques != null && this.cheques.Count() > 0)
                    res = cheques.Sum(mov => mov.monto);
                return res;
            }
        }

        [DisplayName("Monto por pagar")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal montoPorPagar
        {
            get{return Math.Abs(this.montoMovimiento) - Math.Abs(montoPagado);}
        }

        [DisplayName("Pagada")]
        public bool estaPagada { get {
                return this.montoPorPagar <= 0;
            } }

        public string nombreTipoDeduccion { get {
                if (this.tipoDeDeduccion == TipoRetencion.EJIDAL)
                    return NombreRetencion.EJIDAL;
                else if (this.tipoDeDeduccion == TipoRetencion.SANIDAD)
                    return NombreRetencion.SANIDAD;
                else if (this.tipoDeDeduccion == TipoRetencion.OTRO)
                    return NombreRetencion.OTRO;
                else if (this.tipoDeDeduccion == TipoRetencion.ABONO_ANTICIPO)
                    return NombreRetencion.ABONO_ANTICIPO;
                else if (this.tipoDeDeduccion == TipoRetencion.ABONO_ARBOLES)
                    return NombreRetencion.ABONO_ARBOLES;

                return String.Empty;
            }
        }

        public new string concepto
        {
            get
            {
                string abonoTipo = this.nombreTipoDeduccion;
                return String.Format($"RETENCION: {abonoTipo}. CH: {this.liquidacionSemanal.cheque}");
            }
        }

        public enum TipoRetencion
        {
            OTRO, SANIDAD, EJIDAL, ABONO_ANTICIPO, ABONO_ARBOLES
        }

        public static class NombreRetencion
        {
            public const string OTRO = "OTRA DEDUCCION";
            public const string SANIDAD = "Sanidad Vegetal";
            public const string EJIDAL = "2% I.S.R Ejidal";
            public const string ABONO_ANTICIPO = "Abono Anticipos";
            public const string ABONO_ARBOLES = "Abono Arboles";
        }

        public new void ajustarMovimiento()
        {
            this.montoMovimiento *= -1;
            base.ajustarMovimiento();
        }
    }
}