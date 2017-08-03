using System;
using System.Collections.Generic;
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
                string abonoTipo = this.tipoDeDeduccion.ToString();
                if(this.tipoDeDeduccion == TipoRetencion.ABONO_ANTICIPO && this.liquidacionSemanal.abonoAnticipo!=null)
                    abonoTipo = this.liquidacionSemanal.abonoAnticipo.tipoDeMovimientoDeCapital;

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
    }
}