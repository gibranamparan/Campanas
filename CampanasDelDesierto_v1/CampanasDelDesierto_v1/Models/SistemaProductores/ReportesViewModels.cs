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
        { ApplicationDbContext db = new ApplicationDbContext();
            public Productor _productor;

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
            [DisplayName("Interes Devengado")]
            public decimal interesGenerado { get; set; }

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

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo Anterior Cosecha")]
            public decimal adeudoAnteriorCosecha { get; set; }

            [DisplayName("Productor")]
            public string productor
            {
                get
                {
                    return String.Format($"{_productor.numProductor} {_productor.nombreProductor} Zona {_productor.zona}");
                }
            }
            public VMAdeudosRecuperacionReg(Productor productor, TemporadaDeCosecha temporadaActual)
            {
                this._productor = productor;
                //Filtro de movimientos por cosecha
                var movimientos = productor.MovimientosFinancieros
                    .Where(mov => mov.TemporadaDeCosechaID == temporadaActual.TemporadaDeCosechaID);

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

                //Calculo de interes generado a la fecha final de la temporada
                this.interesGenerado = productor.interesTotal(temporadaActual.fechaFin);

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
                //this.saldoPorRecuperar = this.totalAdeudo - this.adeudoRecuperado;
                this.saldoPorRecuperar = movimientosBalanceAnticipos
                    .Where(mov => (mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.CAPITAL 
                        && ((PrestamoYAbonoCapital)mov).tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO) 
                        || mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.VENTA_A_CREDITO)
                        .Sum(mov => mov.montoActivo);

                //Filtro de movs en balance de arboles
                var movimientosBalanceArboles = movimientos
                    .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO);

                //Calculo de compra total de arboles
                this.adeudoArbolitos = Math.Abs(productor.totalDeudaVentaArbolitoPorTemporada(temporadaActual.TemporadaDeCosechaID));

                //Calculo de abono total a deuda por arboles
                this.abonoArbolitosRecuperado = Math.Abs(productor.totalAbonoArbolitoPorTemporada(temporadaActual.TemporadaDeCosechaID));

                //Calculo total de adeudo por recuperar
                this.adeudoPorRecuperarArbolitos = (this.adeudoArbolitos - this.abonoArbolitosRecuperado);

                this.totalRecuperado = this.abonoArbolitosRecuperado + this.adeudoRecuperado;
            }
        }
        public class VMAdeudosRecuperacionDetallado
        {
            ApplicationDbContext db = new ApplicationDbContext();
            private Productor _productor;

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo Anterior Cosecha ")]
            public decimal adeudoAnteriorCosecha { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Anticipos Cosecha Efectivo ")]
            public decimal anticiposCosechaEfectivo { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Anticipos Cosecha Materiales ")]
            public decimal anticiposCosechaMateriales { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Total Adeudos ")]
            public decimal totalAdeudos { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo Recuperado, Cosecha")]
            public decimal adeudoRecuperado { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Saldo por Recuperar")]
            public decimal saldoPorRecuperar { get; set; }

            [DisplayFormat(DataFormatString = "{0:0.000}")]
            [DisplayName("Toneladas Manzanita ")]
            public double toneladasManzanita { get; set; }

            [DisplayFormat(DataFormatString = "{0:0.000}")]
            [DisplayName("Toneladas Mission")]
            public double toneladasMission { get; set; }

            [DisplayFormat(DataFormatString = "{0:0.000}")]
            [DisplayName("Toneladas Obliza ")]
            public double toneladasObliza { get; set; }

            [DisplayFormat(DataFormatString = "{0:0.000}")]
            [DisplayName("Total de Aceituna ")]
            public double totalAceituna { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Pago de Aceituna Cosecha ")]
            public decimal pagoAceitunaCosecha { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo de Arbolitos, Cosecha")]
            public decimal adeudoArbolitosCosecha { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Abono Arbolitos")]
            public decimal abonoArbolitosRecuperado { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo de Arbolitos, Cosecha")]
            public decimal adeudoArbolitos { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Retencion Sanidad Vegetal ")]
            public decimal retencionSanidadVegetal { get; set; }

            [DisplayName("Productor")]
            public string productor
            {
                get
                {
                    return String.Format($"{_productor.numProductor} {_productor.nombreProductor} Zona {_productor.zona}");
                }
            }
            public VMAdeudosRecuperacionDetallado(Productor productor, TemporadaDeCosecha temporadaActual, TemporadaDeCosecha temporadaAnterior)
            {
                this._productor = productor;
                //Filtro de movimientos por cosecha
                var movimientos = productor.MovimientosFinancieros
                    .Where(mov => mov.TemporadaDeCosechaID == temporadaActual.TemporadaDeCosechaID).ToList();

                //Filtro de movimientos capital
                var movimientosCapital = movimientos
                    .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.CAPITAL);

                //Adeudo Anterior cosecha   
                this.adeudoAnteriorCosecha = 0; //Valor por defecto en 0
                if (temporadaAnterior != null) //Si hay temporada anterior, se calcula su balance final
                    this.adeudoAnteriorCosecha = Math.Abs(productor.balanceDeAnticiposEnTemporada(temporadaAnterior.TemporadaDeCosechaID));
                else if(productor.adeudoInicialAnticipos!=null) //Si no, se toma el adeudo anterior registrado para anticipos
                    this.adeudoAnteriorCosecha = Math.Abs(productor.adeudoInicialAnticipos.montoMovimiento);

                //suma del monto de los  movimientos de anticipos por temporda actual
                decimal anticipos = Math.Abs(movimientos
                    .Where(mov=>mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.CAPITAL)
                    .Sum(mon=>mon.montoMovimiento));
                this.anticiposCosechaEfectivo = anticipos;

                //suma del monto de los movimientos de tipo venta a credito por temporada actual
                decimal ventasACredito = Math.Abs(movimientos
                    .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.VENTA_A_CREDITO)
                    .Sum(mon => mon.montoMovimiento));
                this.anticiposCosechaMateriales = ventasACredito;

                //suma total de adeudos
                this.totalAdeudos = anticipos + ventasACredito;

                //Total de adeudo recuperado
                this.adeudoRecuperado = movimientosCapital
                   .Where(mov => ((PrestamoYAbonoCapital)mov).tipoDeMovimientoDeCapital
                       == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO)
                   .Sum(mov => mov.montoMovimiento);

                //Calculo de saldo por recuperar en balance de anticipos
                this.saldoPorRecuperar = this.totalAdeudos - this.adeudoRecuperado;

                //filtro de cosecha pago por producto
                List<PagoPorProducto> movCosecha = movimientos
                    .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.PAGO_POR_PRODUCTO)
                    .Cast<PagoPorProducto>().ToList();

                //se suman las toneladas y se saca el costo de manzanita
                this.toneladasManzanita = movCosecha.Sum(mov=>mov.cantidadProducto1);                
                //se suma las toneladas y se saca el costo de obliza
                this.toneladasObliza = movCosecha.Sum(mov => mov.cantidadProducto2);
                //se suman las toneladas y se saca el costo de mission
                this.toneladasMission = movCosecha.Sum(mov => mov.cantidadProducto3);
                //total de toneladas y se saca el costo de aceituna
                this.totalAceituna = toneladasManzanita + toneladasMission + toneladasObliza;
                List<decimal> pagosCosechaList = new List<decimal>();
                foreach (var ingreso in movCosecha)
                {                    
                    //manzanita, toneladas y costo del producto                    
                    decimal pagoPro1 = ingreso.pagoProducto1;
                    //obliza, toneladas y costo del producto                  
                    decimal pagoPro2 = ingreso.pagoProducto2;
                    //mission, toneladas y costo del producto         
                    decimal pagoPro3 = ingreso.pagoProducto3;
                    /*Se suman el total de las aceitunas para calcular
                      el total de pago de los tres tipos de aceitunas*/
                    decimal pagos = pagoPro1 + pagoPro2 + pagoPro3;
                    pagosCosechaList.Add(pagos);

                }
                this.pagoAceitunaCosecha = pagosCosechaList.Sum();//Se le asigna el total de los pagos del productor

                //filtro de movmientos de balance tipo venta olivo
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

                //Calculo de compra total de arboles
                this.adeudoArbolitos = Math.Abs(movimientosBalanceArboles
                    .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.VENTA_A_CREDITO)
                    .Sum(mov => mov.montoMovimiento));

                //retencion sanidad vegetal
                var movimientosSanidadVegetal = movimientos
                    .Where(mov=>mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.RENTENCION);

                this.retencionSanidadVegetal = (movimientosSanidadVegetal.Sum(mov=>mov.montoMovimiento)) * -1;

            }
        }
    }
}
