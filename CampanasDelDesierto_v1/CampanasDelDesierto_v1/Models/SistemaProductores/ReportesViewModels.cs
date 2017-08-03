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
            
            public VMAdeudosRecuperacionReg(Productor productor, TemporadaDeCosecha temporadaActual)
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

                //Calculo de total de prestamos
                decimal totalPrestamos = Math.Abs(movimientosCapital.
                    Where(mov=>((PrestamoYAbonoCapital)mov).tipoDeMovimientoDeCapital
                        == PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO)
                    .Sum(mov => mov.montoMovimiento));

                //Calculo de total de ventas
                decimal totalVentas = Math.Abs(movimientosBalanceAnticipos
                  .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.VENTA_A_CREDITO)
                  .Sum(mov => mov.montoMovimiento));

                //Calculo de adeudos anteriores
                decimal adeudosAnteriores = Math.Abs(movimientosBalanceAnticipos
                  .Where(mov => mov.isAdeudoInicialMaterial || mov.isAdeudoInicialAnticiposCapital)
                  .Sum(mov => mov.montoMovimiento));

                //Calculo de interes generado a la fecha final de la temporada
                decimal interesTotalGenerado = 0;
                this.interesGenerado = productor.interesTotal(temporadaActual.TemporadaDeCosechaID,
                    DateTime.Today, out interesTotalGenerado);

                //Adeudo total es la suma de prestamos y ventas                
                this.totalAdeudo = totalPrestamos + totalVentas+ adeudosAnteriores + interesTotalGenerado;

                //Suma de todos los abonos a la deuda
                this.adeudoRecuperado = movimientosCapital
                    .Where(mov=>((PrestamoYAbonoCapital)mov).tipoDeMovimientoDeCapital
                        == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO)
                    .Sum(mov => mov.montoMovimiento);

                //Calculo de saldo por recuperar en balance de anticipos
                //this.saldoPorRecuperar = this.totalAdeudo - this.adeudoRecuperado;
                /*this.saldoPorRecuperar = movimientosBalanceAnticipos
                    .Where(mov => (mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.CAPITAL
                        && ((PrestamoYAbonoCapital)mov).tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO)
                        || mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.VENTA_A_CREDITO)
                        .Sum(mov => mov.montoActivo);*/

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
            ApplicationDbContext db = new ApplicationDbContext();
            
            public Productor productor;

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo Anterior Cosecha ")]
            public decimal adeudoAnteriorCosecha { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Anticipos Efectivo ")]
            public decimal anticiposCosechaEfectivo { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Anticipos Materiales ")]
            public decimal anticiposCosechaMateriales { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Total Adeudos ")]
            public decimal totalAdeudos { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Interes Devengado")]
            public decimal interesGenerado { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Interes Devengado")]
            public decimal interesTotalGenerado { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Interes Pagado")]
            public decimal interesPagado { get; set; }

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

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo de Arbolitos, Cosecha")]
            public decimal adeudoArbolitosCosecha { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Abono Arbolitos")]
            public decimal abonoArbolitosRecuperado { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Adeudo Arboles Olivo por Recuperar")]
            public decimal adeudoArbolitosPorRecuperar { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Deducción Sanidad Vegetal ")]
            public decimal retencionSanidadVegetal { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Deducción Ejidal")]
            public decimal retencionEjidal { get; private set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Otras Deducciones")]
            public decimal retencionesOtras { get; private set; }

            public VMAdeudosRecuperacionDetallado() { }
            public VMAdeudosRecuperacionDetallado(Productor productor, TemporadaDeCosecha temporadaActual, TemporadaDeCosecha temporadaAnterior)
            {
                this.productor = productor;
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
                this.anticiposCosechaEfectivo = productor.totalDeudaBalanceAnticiposPorTemporada(temporadaActual.TemporadaDeCosechaID);

                //suma del monto de los movimientos de tipo venta a credito por temporada actual
                this.anticiposCosechaMateriales = Math.Abs(movimientos
                    .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.VENTA_A_CREDITO)
                    .Sum(mon => mon.montoMovimiento));

                //suma total de adeudos
                this.totalAdeudos = this.anticiposCosechaEfectivo + this.anticiposCosechaMateriales;

                //Calculo de interes generado a la fecha final de la temporada
                decimal interesTotal = 0;
                this.interesGenerado = productor.interesTotal(temporadaActual.TemporadaDeCosechaID,temporadaActual.fechaFin, out interesTotal);
                this.interesTotalGenerado = interesTotal;

                //Calculo de interes generado a la fecha final de la temporada
                this.interesPagado = productor.getInteresesPagadosEnLaTemporada(temporadaActual);

                //Total de adeudo recuperado
                this.adeudoRecuperado = productor.getCapitalPagadoEnLaTemporada(temporadaActual);

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
                //se suman las toneladas y se saca el costo de mission
                this.toneladasManzanitaOrg = movCosecha.Sum(mov => mov.cantidadProducto4);
                //se suman las toneladas y se saca el costo de mission
                this.toneladasOblizaOrg = movCosecha.Sum(mov => mov.cantidadProducto5);
                //se suman las toneladas y se saca el costo de mission
                this.toneladasMissionOrg = movCosecha.Sum(mov => mov.cantidadProducto6);
                //total de toneladas y se saca el costo de aceituna
                this.totalAceituna = toneladasManzanita + toneladasMission + toneladasObliza+
                    toneladasManzanitaOrg+ toneladasOblizaOrg+ toneladasMissionOrg;
                List<decimal> pagosCosechaList = new List<decimal>();

                //CALCULO DE PAGO TOTAL
                //manzanita, toneladas y costo del producto                    
                decimal pagoPro1 = movCosecha.Sum(mov=>mov.pagoProducto1);
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

                /**********BALANCE DE COMPRAS DE ARBOLES***********/
                //filtro de movmientos de balance tipo venta olivo
                var movimientosBalanceArboles = movimientos
                    .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO);

                //Calculo de compra total de arboles
                this.adeudoArbolitosCosecha = Math.Abs(productor.totalDeudaVentaArbolitoPorTemporada(temporadaActual.TemporadaDeCosechaID));

                //Calculo de abono total a deuda por arboles
                this.abonoArbolitosRecuperado = Math.Abs(productor.totalAbonoArbolitoPorTemporada(temporadaActual.TemporadaDeCosechaID));

                //Calculo total de adeudo por recuperar
                this.adeudoArbolitosPorRecuperar = (this.adeudoArbolitosCosecha - this.abonoArbolitosRecuperado);

                //retencion sanidad vegetal
                var movimientosRetenciones = movimientos
                    .Where(mov=>mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.RENTENCION).Cast<Retencion>();

                this.retencionSanidadVegetal = Math.Abs(movimientosRetenciones.Where(mov=>mov.tipoDeDeduccion == Retencion.TipoRetencion.SANIDAD).Sum(mov=>mov.montoMovimiento));
                this.retencionEjidal = Math.Abs(movimientosRetenciones.Where(mov => mov.tipoDeDeduccion == Retencion.TipoRetencion.EJIDAL).Sum(mov => mov.montoMovimiento));
                this.retencionesOtras = Math.Abs(movimientosRetenciones.Where(mov => mov.tipoDeDeduccion == Retencion.TipoRetencion.OTRO).Sum(mov => mov.montoMovimiento));
            }

            public static VMAdeudosRecuperacionDetallado calcularTotales(List<VMAdeudosRecuperacionDetallado> datos)
            {
                VMAdeudosRecuperacionDetallado res = new VMAdeudosRecuperacionDetallado();
                res.abonoArbolitosRecuperado = datos.Sum(mov => mov.abonoArbolitosRecuperado);
                res.adeudoAnteriorCosecha = datos.Sum(mov => mov.adeudoAnteriorCosecha);
                res.adeudoArbolitosCosecha= datos.Sum(mov => mov.adeudoArbolitosCosecha);
                res.adeudoArbolitosPorRecuperar= datos.Sum(mov => mov.adeudoArbolitosPorRecuperar);
                res.adeudoRecuperado= datos.Sum(mov => mov.adeudoRecuperado);
                res.anticiposCosechaEfectivo= datos.Sum(mov => mov.anticiposCosechaEfectivo);
                res.interesGenerado = datos.Sum(mov => mov.interesGenerado);
                res.interesPagado = datos.Sum(mov => mov.interesPagado);
                res.anticiposCosechaMateriales = datos.Sum(mov => mov.anticiposCosechaMateriales);
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
    }
}
