using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models.SistemaProductores
{
    public class AdeudoInicial : MovimientoFinanciero
    {
        [DisplayName("Balance Adeudado")]
        public TipoDeBalance balanceAdeudado { get; set; }

        [DisplayName("Concepto")]
        public new string concepto
        {
            get
            {
                return this.nombreDeMovimiento;
            }
        }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [DisplayName("Interes Inicial")]
        public decimal interesInicial { get; set; }

        public void ajustarMovimiento(Productor productor, TemporadaDeCosecha temporada, TipoDeBalance tipoBalance)
        {
            this.montoMovimiento *= -1;
            this.balanceAdeudado = tipoBalance;
            this.idProductor = productor.idProductor;
            this.fechaMovimiento = productor.fechaIntegracion;
            this.TemporadaDeCosechaID = temporada.TemporadaDeCosechaID;
            //base.ajustarMovimiento();
        }

        public static System.Data.Entity.EntityState determinarEstadoMovimiento(AdeudoInicial adeudo)
        {
            MovimientoFinanciero temp = new MovimientoFinanciero();
            temp.montoMovimiento = adeudo.montoMovimiento + adeudo.interesInicial;
            temp.idMovimiento = adeudo.idMovimiento;
            return MovimientoFinanciero.determinarEstadoMovimiento(temp);
        }
    }
}