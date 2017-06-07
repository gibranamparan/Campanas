using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class Deduccion : MovimientoFinanciero
    {
        public enum TipoDeduccion
        {
            OTRO, SANIDAD, EJIDAL
        }

        public TipoDeduccion tipoDeDeduccion { get; set; }

        public string nombreTipoDeduccion { get {
                if (this.tipoDeDeduccion == TipoDeduccion.EJIDAL)
                    return "2% I.S.R Ejidal";
                else if (this.tipoDeDeduccion == TipoDeduccion.SANIDAD)
                    return "Garantía de Sanidad Vegetal";
                else if(this.tipoDeDeduccion == TipoDeduccion.OTRO)
                    return "OTRA DEDUCCION";

                return String.Empty;
            }
        }

        public new void ajustarMovimiento()
        {
            this.montoMovimiento *= -1;
            base.ajustarMovimiento();
        }

        public Deduccion() { }
        public Deduccion(LiquidacionSemanal emisionDeCheque, decimal monto, TipoDeduccion td)
        {
            this.montoMovimiento = monto;
            this.fechaMovimiento = emisionDeCheque.fechaMovimiento;
            this.TemporadaDeCosechaID = emisionDeCheque.TemporadaDeCosechaID;
            this.idProductor = emisionDeCheque.idProductor;
            this.tipoDeDeduccion = td;
        }
    }
}