using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class ReportesViewModels
    {
        public class VMAdeudosRecuperacionReg
        {
            private Productor _productor;

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Total de Adeudos, Cosecha")]
            public decimal totalAdeudo { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo Recuperado, Cosecha")]
            public decimal adeudoRecuperado { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Saldo por Recuperar")]
            public decimal saldoPorRecuperar { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo de Arbolitos, Cosecha")]
            public decimal adeudoArbolitos { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Abono Arbolitos Recuperado")]
            public decimal abonoArbolitosRecuperado{ get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Saldo por Recuperar de Arbolitos")]
            public decimal adeudoPorRecuperarArbolitos { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Total Recuperado")]
            public decimal totalRecuperado { get; set; }

            [DisplayName("Productor")]
            public string productor
            {
                get
                {
                    return String.Format($"{_productor.numProductor} {_productor.nombreProductor} Zona {_productor.zona}");
                }
            }
            public VMAdeudosRecuperacionReg(Productor productor, int temporadaID)
            {
                this._productor = productor;
                //Filtro de movimientos por cosecha
                var movimientos = productor.MovimientosFinancieros
                    .Where(mov => mov.TemporadaDeCosechaID == temporadaID);

                //Filtro de movimientos dentro del balance de anticipos
                var movimientosBalanceAnticipos = movimientos
                    .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS)
                    .ToList();

                //Filtro de movs tipo capital
                var movimientosCapital = movimientosBalanceAnticipos
                    .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.CAPITAL);

                //Calculo de total de prestamos
                decimal totalPrestamos = movimientosCapital.
                    Where(mov=>((PrestamoYAbonoCapital)mov).tipoDeMovimientoDeCapital
                        == PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO)
                    .Sum(mov => mov.montoMovimiento);

                //Calculo de total de ventas
                decimal totalVentas = movimientosBalanceAnticipos
                    .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.VENTA_A_CREDITO)
                    .Sum(mov => mov.montoMovimiento);

                //Adeudo total es la suma de prestamos y ventas
                this.totalAdeudo = -(totalPrestamos + totalVentas);

                //Suma de todos los abonos a la deuda
                this.adeudoRecuperado = movimientosCapital
                    .Where(mov=>((PrestamoYAbonoCapital)mov).tipoDeMovimientoDeCapital
                        == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO)
                    .Sum(mov => mov.montoMovimiento);

                //Calculo de saldo por recuperar en balance de anticipos
                this.saldoPorRecuperar = this.totalAdeudo - this.adeudoRecuperado;

                //Filtro de movs en balance de arboles
                var movimientosBalanceArboles = movimientos
                    .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO);

                //Calculo de compra total de arboles
                this.adeudoArbolitos = Math.Abs(movimientosBalanceArboles
                    .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.VENTA_A_CREDITO)
                    .Sum(mov => mov.montoMovimiento));

                //Calculo de abono total a deuda por arboles
                this.abonoArbolitosRecuperado = Math.Abs(movimientosBalanceArboles
                    .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.CAPITAL)
                    .Sum(mov => mov.montoMovimiento));

                //Calculo total de adeudo por recuperar
                this.adeudoPorRecuperarArbolitos = (this.adeudoArbolitos - this.abonoArbolitosRecuperado);

                this.totalRecuperado = this.abonoArbolitosRecuperado + this.adeudoRecuperado;
            }
        }
    }
}