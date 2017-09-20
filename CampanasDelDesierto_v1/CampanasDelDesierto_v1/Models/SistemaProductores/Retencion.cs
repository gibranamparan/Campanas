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

        public static string getNombreTipoRetencion(TipoRetencion tipo)
        {
            if (tipo == TipoRetencion.EJIDAL)
                return NombreRetencion.EJIDAL;
            else if (tipo == TipoRetencion.SANIDAD)
                return NombreRetencion.SANIDAD;
            else if (tipo == TipoRetencion.OTRO)
                return NombreRetencion.OTRO;
            else if (tipo == TipoRetencion.ABONO_ANTICIPO)
                return NombreRetencion.ABONO_ANTICIPO;
            else if (tipo == TipoRetencion.ABONO_ARBOLES)
                return NombreRetencion.ABONO_ARBOLES;
            else if (tipo == TipoRetencion.NINGUNO)
                return NombreRetencion.NINGUNO;

            return String.Empty;
        }

        public new string concepto
        {
            get
            {
                string nombreRetencion = Retencion.getNombreTipoRetencion(this.tipoDeDeduccion);
                return String.Format($"RETENCION: {nombreRetencion}. CH: {this.liquidacionSemanal.cheque}");
            }
        }

        public enum TipoRetencion
        {
            OTRO, SANIDAD, EJIDAL, ABONO_ANTICIPO, ABONO_ARBOLES, NINGUNO
        }

        public static class NombreRetencion
        {
            public const string OTRO = "Otras Deducciones";
            public const string SANIDAD = "Sanidad Vegetal";
            public const string EJIDAL = "2% I.S.R Ejidal";
            public const string ABONO_ANTICIPO = "Abono Anticipos";
            public const string ABONO_ARBOLES = "Abono Árboles";
            public const string NINGUNO = "Ninguna";
        }

        public new void ajustarMovimiento()
        {
            this.montoMovimiento *= -1;
            base.ajustarMovimiento();
        }
    }
}