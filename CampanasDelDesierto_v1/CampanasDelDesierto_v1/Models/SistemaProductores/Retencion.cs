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
                    return "2% I.S.R Ejidal";
                else if (this.tipoDeDeduccion == TipoRetencion.SANIDAD)
                    return "Garantía de Sanidad Vegetal";
                else if(this.tipoDeDeduccion == TipoRetencion.OTRO)
                    return "OTRA DEDUCCION";

                return String.Empty;
            }
        }

        public enum TipoRetencion
        {
            OTRO, SANIDAD, EJIDAL, ABONO
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