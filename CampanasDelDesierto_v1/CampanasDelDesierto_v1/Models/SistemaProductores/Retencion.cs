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
                else if (this.tipoDeDeduccion == TipoRetencion.ABONO)
                    return NombreRetencion.ABONO;

                return String.Empty;
            }
        }

        public new string concepto
        {
            get
            {
                return String.Format($"RETENCION: {this.tipoDeDeduccion}. CH: {this.liquidacionSemanal.cheque}");
            }
        }

        public enum TipoRetencion
        {
            OTRO, SANIDAD, EJIDAL, ABONO
        }

        public static class NombreRetencion
        {
            public const string OTRO = "OTRA DEDUCCION";
            public const string SANIDAD = "Sanidad Vegetal";
            public const string EJIDAL = "2% I.S.R Ejidal";
            public const string ABONO = "Abono a Anticipos";
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