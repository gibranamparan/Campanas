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
        public AdeudoInicial(VMMovimientoBalanceAnticipos.VMBalanceAnticiposTotales totales, 
            TemporadaDeCosecha temporada, Productor productor, bool isVentas = false)
        {
            if (isVentas) { //Deuda inicial de venta a credito
                this.montoMovimiento =- (Math.Abs(totales.ventasACredito) + Math.Abs(totales.deudaCapitalInicial));
                this.interesInicial = 0;
            }
            else //Deuda inicial de anticipos de capital
            {
                this.montoMovimiento =-( Math.Abs(totales.anticiposEfectivo) + Math.Abs(totales.deudaVentasInicial));
                this.interesInicial = totales.interes;
            }

            this.idProductor = productor.idProductor;
            this.Productor = productor;
            this.TemporadaDeCosechaID = temporada.TemporadaDeCosechaID;
            this.temporadaDeCosecha = temporada;
            this.fechaMovimiento = temporada.fechaFin;
            this.isVentas = isVentas;
        }

        public AdeudoInicial()
        {
        }

        [DisplayName("Balance Adeudado")]
        public TipoDeBalance balanceAdeudado { get; set; }

        public Boolean? isVentas { get; set; }

        public bool isRegistradoInicialmenteEnProductor
            {get{return this.idMovimiento > 0;}}

        /// <summary>
        /// Muestra el nombre del tipo de adeudo inicial
        /// </summary>
        [DisplayName("Concepto")]
        public new string concepto { get {
                string res = string.Empty;
                res = this.nombreDeMovimiento;
                if (this.isAdeudoInicialAnticiposCapital)
                    res += " ANTICIPOS";
                else if (this.isAdeudoInicialMaterial)
                    res += " COMPRA MATERIAL";
                else if (this.isAdeudoInicialVentaOlivo)
                    res += " COMPRA ARBOLES OLIVO";
                return res;
            } }

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