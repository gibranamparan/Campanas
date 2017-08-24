using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VMBalanceAnticiposTotales = CampanasDelDesierto_v1.Models.MovimientoFinanciero.VMMovimientoBalanceAnticipos.VMBalanceAnticiposTotales;

namespace CampanasDelDesierto_v1.Models
{
    public class ReportesViewModels
    {
        public class VMAdeudosRecuperacionReg
        {
            public Productor productor;

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
            [DisplayName("Adeudo Arboles Olivo, Cosecha")]
            public decimal adeudoArbolitos { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Abono Arboles Olivo")]
            public decimal abonoArbolitosRecuperado{ get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Saldo por Recuperar Arboles Olivo")]
            public decimal adeudoPorRecuperarArbolitos { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Total Recuperado")]
            public decimal totalRecuperado { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo Anterior Cosecha")]
            public decimal adeudoAnteriorCosecha { get; set; }
            
            public VMAdeudosRecuperacionReg(Productor productor, TemporadaDeCosecha temporadaActual, TemporadaDeCosecha temporadaAnterior)
            {
                this.productor = productor;
                //Filtro de movimientos por cosecha
                var movimientos = productor.MovimientosFinancieros
                    .Where(mov => mov.TemporadaDeCosechaID == temporadaActual.TemporadaDeCosechaID);

                //Filtro de movimientos dentro del balance de anticipos
                var movimientosBalanceAnticipos = movimientos
                    .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS
                    || mov.isAdeudoInicialMaterial || mov.isAdeudoInicialAnticiposCapital)
                    .ToList();

                //Filtro de movs tipo capital o anticipos
                var movimientosCapital = movimientosBalanceAnticipos
                    .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.CAPITAL);

                //Se genera el reporte de movimientos de anticipos en la temporada calculando intereses a la fecha actual
                IEnumerable<MovimientoFinanciero.VMMovimientoBalanceAnticipos> reporteAnticipos =
                    productor.generarReporteAnticiposConIntereses(DateTime.Today, temporadaActual,temporadaAnterior);

                //Se calculan los montos totales del reporte.
                MovimientoFinanciero.VMMovimientoBalanceAnticipos.VMBalanceAnticiposTotales totales =
                    new MovimientoFinanciero.VMMovimientoBalanceAnticipos.VMBalanceAnticiposTotales(reporteAnticipos);

                //Adeudo total es la suma de prestamos y ventas                
                this.totalAdeudo = Math.Abs(totales.anticipo) + totales.interes /*- totales.deudaCapitalInicial - totales.deudaInteresInicial*/;

                //Suma de todos los abonos a la deuda
                this.adeudoRecuperado = totales.abonoCapital + totales.abonoInteres;

                this.saldoPorRecuperar = this.totalAdeudo - this.adeudoRecuperado;

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
            public Productor productor;

            /*Deudas------------*/
            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo Cosecha Anterior")]
            public decimal adeudoAnteriorCosecha { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo Interes Cosecha Anterior")]
            public decimal adeudoInteresAnteriorCosecha { get; set; }

            [DisplayName("Adeudo Venta Materiales Cosecha Anterior")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal adeudoVentaCreditoAnteriorCosecha { get; set; }

            [DisplayName("Adeudo Venta Arboles Anterior Cosecha ")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal adeudoVentaArbolesAnteriorCosecha { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Anticipos de Efectivo")]
            public decimal anticiposEfectivo { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Ventas a Credito")]
            public decimal ventasCredito { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Interes")]
            public decimal interes { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Total Adeudos ")]
            public decimal totalAdeudos { get; set; }
            /*FIN DE DEUDAS----------*/

            /*Abonos -----*/
            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Interes abonado")]
            public decimal interesAbonado { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo Recuperado")]
            public decimal adeudoRecuperado { get; set; } //En ventas a credito y anticipos de efectivo
            /*FIN DE ABONOS ------------*/

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Saldo por Recuperar")]
            public decimal saldoPorRecuperar { get; set; } /*TOTAL POR RECUPERAR*/

            /*Conteo de cosechas*/
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
            [DisplayName("Toneladas Manz. Org. ")]
            public double toneladasManzanitaOrg { get; set; }

            [DisplayFormat(DataFormatString = "{0:0.000}")]
            [DisplayName("Toneladas Obliza Org. ")]
            public double toneladasOblizaOrg { get; set; }

            [DisplayFormat(DataFormatString = "{0:0.000}")]
            [DisplayName("Toneladas Mission Org. ")]
            public double toneladasMissionOrg { get; set; }

            [DisplayFormat(DataFormatString = "{0:0.000}")]
            [DisplayName("Total de Aceituna ")]
            public double totalAceituna { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Pago de Aceituna Cosecha ")]
            public decimal pagoAceitunaCosecha { get; set; }
            /*FIN DE COSECHAS*/

            /*Ventas de arbolitos -------------*/
            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo de Arbolitos")]
            public decimal adeudoArboles { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Abono Arbolitos")]
            public decimal abonoArbolitos { get; set; }

            /// <summary>
            /// Adeudo remanente de la temporada actual por concepto de arboles de olivo.
            /// </summary>
            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo Arboles Olivo por Recuperar")]
            public decimal adeudoArbolitosPorRecuperar { get; set; }

            /// <summary>
            /// Monto total por ventas en las que se hizo compras de polen.
            /// </summary>
            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Venta total de polen")]
            public decimal ventaTotalPolen { get; private set; }
            /*FIN DE ARBOLITOS ---------------*/

            /*Retenciones------------*/
            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Deducción Sanidad Vegetal ")]
            public decimal retencionSanidadVegetal { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Deducción Ejidal")]
            public decimal retencionEjidal { get; private set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Otras Deducciones")]
            public decimal retencionesOtras { get; private set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Saldo Pendiente Anticipos")]
            public decimal saldoPendienteAnticipos { get; private set; }

            /*FIN DE RETENCINOES--------------*/

            private void calcularAdedeudos(TemporadaDeCosecha temporadaActual, TemporadaDeCosecha temporadaAnterior,
                VMBalanceAnticiposTotales totales)
            {           
                this.adeudoAnteriorCosecha = Math.Abs(totales.deudaCapitalInicial);
                this.adeudoInteresAnteriorCosecha = Math.Abs(totales.deudaInteresInicial);
                this.adeudoVentaCreditoAnteriorCosecha = Math.Abs(totales.deudaVentasInicial);

                //this.anticiposEfectivo = Math.Abs(totales.anticiposEfectivo)-this.adeudoAnteriorCosecha;
                this.anticiposEfectivo = Math.Abs(totales.anticiposEfectivo);
                this.ventasCredito = Math.Abs(totales.ventasACredito)-totales.deudaVentasInicial;
                this.interes = totales.interes - this.adeudoInteresAnteriorCosecha;
                this.totalAdeudos = this.anticiposEfectivo + this.ventasCredito + this.interes 
                    + this.adeudoInteresAnteriorCosecha + adeudoAnteriorCosecha + adeudoVentaCreditoAnteriorCosecha;
            }
            
            private void calcularVentasDeOlivo(List<MovimientoFinanciero> movimientos, TemporadaDeCosecha temporadaActual,
                VMBalanceAnticiposTotales totales)
            {
                //filtro de movmientos de balance tipo venta olivo
                var movimientosBalanceArboles = movimientos
                    .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO);

                //Calculo de compra total de arboles
                this.adeudoArboles = Math.Abs(productor.totalDeudaVentaArbolitoPorTemporada(temporadaActual.TemporadaDeCosechaID));

                //Adeudo inicial por venta de arboles
                this.adeudoVentaArbolesAnteriorCosecha = totales.deudaVentasArbolesInicial;

                //Calculo de abono total a deuda por arboles
                this.abonoArbolitos = Math.Abs(productor.totalAbonoArbolitoPorTemporada(temporadaActual.TemporadaDeCosechaID));

                //Calculo total de adeudo por recuperar
                this.adeudoArbolitosPorRecuperar = this.adeudoArboles < this.abonoArbolitos?0:(this.adeudoArboles - this.abonoArbolitos);

                var ventasPolen = movimientos.Where(mov => mov.isVentaDeMaterial).ToList()
                    .Where(mov => (VentaACredito.isVentaPolen(((VentaACredito)mov).ComprasProductos)));
                this.ventaTotalPolen = Math.Abs(ventasPolen.Sum(mov => mov.montoMovimiento));
            }

            private void calcularPagoPorCosecha(List<MovimientoFinanciero> movimientos)
            {
                //filtro de cosecha pago por producto
                List<PagoPorProducto> movCosecha = movimientos
                    .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.PAGO_POR_PRODUCTO)
                    .Where(mov => ((PagoPorProducto)mov).yaLiquidado)
                    .Cast<PagoPorProducto>().ToList();

                //se suman las toneladas y se saca el costo de manzanita
                this.toneladasManzanita = movCosecha.Sum(mov => mov.cantidadProducto1);
                //se suma las toneladas y se saca el costo de obliza
                this.toneladasObliza = movCosecha.Sum(mov => mov.cantidadProducto2);
                //se suman las toneladas y se saca el costo de mission
                this.toneladasMission = movCosecha.Sum(mov => mov.cantidadProducto3);
                //se suman las toneladas y se saca el costo de mission
                this.toneladasManzanitaOrg = movCosecha.Sum(mov => mov.cantidadProducto4);
                //se suman las toneladas y se saca el costo de mission
                this.toneladasOblizaOrg = movCosecha.Sum(mov => mov.cantidadProducto5);
                //se suman las toneladas y se saca el costo de mission
                this.toneladasMissionOrg = movCosecha.Sum(mov => mov.cantidadProducto6);
                //total de toneladas y se saca el costo de aceituna
                this.totalAceituna = toneladasManzanita + toneladasMission + toneladasObliza +
                    toneladasManzanitaOrg + toneladasOblizaOrg + toneladasMissionOrg;
                List<decimal> pagosCosechaList = new List<decimal>();

                //CALCULO DE PAGO TOTAL
                //manzanita, toneladas y costo del producto                    
                decimal pagoPro1 = movCosecha.Sum(mov => mov.pagoProducto1);
                //obliza, toneladas y costo del producto                  
                decimal pagoPro2 = movCosecha.Sum(mov => mov.pagoProducto2);
                //mission, toneladas y costo del producto         
                decimal pagoPro3 = movCosecha.Sum(mov => mov.pagoProducto3);
                //manzanita org, toneladas y costo del producto                    
                decimal pagoPro4 = movCosecha.Sum(mov => mov.pagoProducto4);
                //obliza, toneladas y costo del producto                  
                decimal pagoPro5 = movCosecha.Sum(mov => mov.pagoProducto5);
                //mission, toneladas y costo del producto         
                decimal pagoPro6 = movCosecha.Sum(mov => mov.pagoProducto6);
                //Se le asigna el total de los pagos del productor
                this.pagoAceitunaCosecha = pagoPro1 + pagoPro2 + pagoPro3 + pagoPro4 + pagoPro5 + pagoPro6;
            }

            private void calcularRetenciones(List<MovimientoFinanciero> movimientos)
            {

                var movimientosRetenciones = movimientos
                    .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.RENTENCION).Cast<Retencion>();

                this.retencionSanidadVegetal = Math.Abs(movimientosRetenciones.Where(mov => mov.tipoDeDeduccion == Retencion.TipoRetencion.SANIDAD).Sum(mov => mov.montoMovimiento));
                this.retencionEjidal = Math.Abs(movimientosRetenciones.Where(mov => mov.tipoDeDeduccion == Retencion.TipoRetencion.EJIDAL).Sum(mov => mov.montoMovimiento));
                this.retencionesOtras = Math.Abs(movimientosRetenciones.Where(mov => mov.tipoDeDeduccion == Retencion.TipoRetencion.OTRO).Sum(mov => mov.montoMovimiento));
            }

            public VMAdeudosRecuperacionDetallado() { }
            public VMAdeudosRecuperacionDetallado(Productor productor,
                TemporadaDeCosecha temporadaActual, TemporadaDeCosecha temporadaAnterior)
            {
                this.productor = productor;
                //Filtro de movimientos por cosecha
                var movimientos = productor.MovimientosFinancieros
                    .Where(mov => mov.TemporadaDeCosechaID == temporadaActual.TemporadaDeCosechaID).ToList();
                
                //Se genera el reporte de movimientos de anticipos en la temporada calculando intereses a la fecha actual
                IEnumerable<MovimientoFinanciero.VMMovimientoBalanceAnticipos> reporteAnticipos =
                    productor.generarReporteAnticiposConIntereses(DateTime.Today, temporadaActual, temporadaAnterior);

                //Se calculan los montos totales del reporte.
                VMBalanceAnticiposTotales totales = new VMBalanceAnticiposTotales(reporteAnticipos);
                
                //ADEUDOS SE CALCULAN
                calcularAdedeudos(temporadaActual, temporadaAnterior, totales);
                
                //ABONOS Y SALDO POR RECUPERAR
                //Calculo de interes generado a la fecha final de la temporada
                this.interesAbonado = totales.abonoInteres;
                //Total de adeudo recuperado
                this.adeudoRecuperado = totales.abonoCapital;
                //Calculo de saldo por recuperar en balance de anticipos
                this.saldoPorRecuperar = this.totalAdeudos - (this.adeudoRecuperado+this.interesAbonado);

                this.saldoPendienteAnticipos = this.totalAdeudos < (this.adeudoRecuperado + this.interesAbonado)?0:
                    this.totalAdeudos - (this.adeudoRecuperado + this.interesAbonado);

                //PAGOS POR COSECHA
                calcularPagoPorCosecha(movimientos);

                //VENTAS DE OLIVO
                calcularVentasDeOlivo(movimientos, temporadaActual, totales);

                //retencion sanidad vegetal
                calcularRetenciones(movimientos);
            }

            public static VMAdeudosRecuperacionDetallado calcularTotales(List<VMAdeudosRecuperacionDetallado> datos)
            {
                VMAdeudosRecuperacionDetallado res = new VMAdeudosRecuperacionDetallado();
                res.abonoArbolitos = datos.Sum(mov => mov.abonoArbolitos);
                res.adeudoAnteriorCosecha = datos.Sum(mov => mov.adeudoAnteriorCosecha);
                res.adeudoInteresAnteriorCosecha = datos.Sum(mov => mov.adeudoInteresAnteriorCosecha);
                res.adeudoArboles= datos.Sum(mov => mov.adeudoArboles);
                res.adeudoArbolitosPorRecuperar= datos.Sum(mov => mov.adeudoArbolitosPorRecuperar);
                res.adeudoRecuperado= datos.Sum(mov => mov.adeudoRecuperado);
                res.anticiposEfectivo= datos.Sum(mov => mov.anticiposEfectivo);
                res.interes = datos.Sum(mov => mov.interes);
                res.interesAbonado = datos.Sum(mov => mov.interesAbonado);
                res.ventasCredito = datos.Sum(mov => mov.ventasCredito);
                res.pagoAceitunaCosecha= datos.Sum(mov => mov.pagoAceitunaCosecha);
                res.retencionEjidal= datos.Sum(mov => mov.retencionEjidal);
                res.retencionesOtras= datos.Sum(mov => mov.retencionesOtras);
                res.retencionSanidadVegetal= datos.Sum(mov => mov.retencionSanidadVegetal);
                res.saldoPorRecuperar= datos.Sum(mov => mov.saldoPorRecuperar);
                res.toneladasManzanita= datos.Sum(mov => mov.toneladasManzanita);
                res.toneladasManzanitaOrg = datos.Sum(mov => mov.toneladasManzanitaOrg);
                res.toneladasMission = datos.Sum(mov => mov.toneladasMission);
                res.toneladasMissionOrg= datos.Sum(mov => mov.toneladasMissionOrg);
                res.toneladasObliza= datos.Sum(mov => mov.toneladasObliza);
                res.toneladasOblizaOrg = datos.Sum(mov => mov.toneladasOblizaOrg);
                res.totalAceituna= datos.Sum(mov => mov.totalAceituna);
                res.totalAdeudos = datos.Sum(mov => mov.totalAdeudos);

                return res;
            }
        }
        public class VMLiquidacionFinal : VMAdeudosRecuperacionDetallado
        {
            public VMLiquidacionFinal(Productor productor, TemporadaDeCosecha temporadaActual, TemporadaDeCosecha temporadaAnterior) 
                : base(productor, temporadaActual, temporadaAnterior)
            {
            }
        }
    }
}
