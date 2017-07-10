﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static CampanasDelDesierto_v1.Models.PrestamoYAbonoCapital;

namespace CampanasDelDesierto_v1.Models
{
    public class MovimientoFinanciero
    {
        [Key]
        public int idMovimiento { get; set; }

        /// <summary>
        /// Representa el movimiento de capital neto para esta instancia.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Monto (USD)")]
        public decimal montoMovimiento { get; set; }

        /// <summary>
        /// Almacena el balance correspondiente al hacerse este movimiento con respecto a todos los movimientos
        /// introducidos dentro del mismo balance ordenados por orden cronológico. Acceder a tipoDeBalance para
        /// determinar dentro de que tipo entra esta instancia.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Balance (USD)")]
        public decimal balance { get; set; }

        /// <summary>
        /// Fecha en la que se realizó el movimiento.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime fechaMovimiento { get; set; }

        /// <summary>
        /// Llave primaria del productor al que le responde la instancia del movimiento.
        /// Todos los movimientos financieros le corresponden a un productor.
        /// </summary>
        public int idProductor { get; set; }

        public Boolean isAbonoCapital
        {
            get
            {
                return this.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.CAPITAL &&
                ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO;
            }
        }

        /// <summary>
        /// Instancia virtual del productor al que le responde la instancia del movimiento.
        /// A una instancia de productor le corresponden muchos movimientos.
        /// </summary>
        public virtual Productor Productor { get; set; }

        //Cada movimiento entra dentro de un periodo de cosecha previamente aperturado
        public int TemporadaDeCosechaID { get; set; }
        public virtual TemporadaDeCosecha temporadaDeCosecha { get; set; }

        /// <summary>
        /// Abonos recibidos a este prestamo
        /// </summary>
        [InverseProperty("prestamo")]
        public virtual ICollection<Prestamo_Abono> abonosRecibidos { get; set; }

        /// <summary>
        /// Propiedad padre de concepto la cual determina que concepto desplegar segun el tipo de movimiento
        /// al que corresponda la presente instancia.
        /// </summary>
        [DisplayName("Concepto")]
        public string concepto { get {
                string res = String.Empty;
                if (this.getTypeOfMovement() == TypeOfMovements.CAPITAL)
                    res = ((PrestamoYAbonoCapital)this).concepto;
                else if (this.getTypeOfMovement() == TypeOfMovements.LIQUIDACION)
                    res = ((LiquidacionSemanal)this).concepto;
                else if (this.getTypeOfMovement() == TypeOfMovements.PAGO_POR_PRODUCTO)
                    res = ((PagoPorProducto)this).concepto;
                else if (this.getTypeOfMovement() == TypeOfMovements.RENTENCION)
                    res = ((Retencion)this).concepto;
                else if (this.getTypeOfMovement() == TypeOfMovements.VENTA_A_CREDITO)
                    res = ((VentaACredito)this).concepto;

                return string.IsNullOrEmpty(res)?String.Empty:res;
            } }

        /// <summary>
        /// Despliega el nombre en String del tipo de movimiento al que corresponde la presente instancia.
        /// </summary>
        [Display(Name = "Tipo")]
        public string nombreDeMovimiento
        {
            get
            {
                TypeOfMovements tom = this.getTypeOfMovement();
                if (tom == TypeOfMovements.CAPITAL) {
                    return ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital;
                }
                else if (tom == TypeOfMovements.PAGO_POR_PRODUCTO)
                    return "INGRESO DE COSECHA";
                else if (tom == TypeOfMovements.VENTA_A_CREDITO)
                    return "VENTA A CREDITO";
                else if (tom == TypeOfMovements.LIQUIDACION)
                    return "LIQUIDACION";
                else if (tom == TypeOfMovements.RENTENCION) {
                    return "RETENCION" ;
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Enumeracion de tipos de balances bajo los que se que agrupan los diferentes movimientos financieros.
        /// </summary>
        public enum TipoDeBalance
        {
            NONE, CAPITAL_VENTAS, VENTA_OLIVO, MOV_LIQUIDACION
        }

        /// <summary>
        /// Arroja el tipo de balance correspondiente a esta instancia segun su tipo de movimiento y
        /// características particulares. Ver los metodos isVentaDeOlivo(), isAbonoOPrestamo() y 
        /// isMovimientoDeLiquidacion().
        /// </summary>
        public TipoDeBalance tipoDeBalance { get
            {
                if (this.isAbonoOPrestamo())
                {
                    return TipoDeBalance.CAPITAL_VENTAS;
                } else if (this.isMovimientoDeLiquidacion())
                {
                    return TipoDeBalance.MOV_LIQUIDACION;
                } else if (this.isVentaDeOlivo())
                {
                    return TipoDeBalance.VENTA_OLIVO;
                }
                return TipoDeBalance.NONE;
            } }

        /// <summary>
        /// Enumerador de tipos de movimientos
        /// </summary>
        public enum TypeOfMovements
        {
            NONE,
            PAGO_POR_PRODUCTO,
            CAPITAL,
            VENTA_A_CREDITO,
            LIQUIDACION, 
            RENTENCION
        };

        public MovimientoFinanciero() { }

        public decimal totalLiquidadoPrestamo
        {
            get
            {
                return this.abonosRecibidos != null ? this.abonosRecibidos.Sum(mov => mov.monto) : 0;
            }
        }

        /// <summary>
        /// Si el movimiento es de tipo abono a capital, se
        /// determina la cantidad de dinero distribuida en los anticipos
        /// </summary>
        public decimal totalGastadoAbono
        {
            get
            {
                if (this.getTypeOfMovement() == TypeOfMovements.CAPITAL
                    && ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital
                        == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO)
                {
                    return ((PrestamoYAbonoCapital)this).prestamosAbonados != null ? 
                        ((PrestamoYAbonoCapital)this).prestamosAbonados.Sum(mov => mov.monto) : 0;
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// Indica el monto faltante por saldar para un anticipo o el monto
        /// disponible para seguir abonando en el caso de un abono.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C}")]
        [DisplayName("Saldo Capital")]
        public decimal montoActivo
        {
            get
            {
                decimal suma = 0;
                if (this.getTypeOfMovement() == TypeOfMovements.CAPITAL)
                {
                    suma = ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO
                    ? this.totalGastadoAbono :
                    ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO ?
                    this.totalLiquidadoPrestamo : 0;
                }
                else if (this.getTypeOfMovement() == TypeOfMovements.VENTA_A_CREDITO)
                {
                    suma = this.totalLiquidadoPrestamo;
                }

                return Math.Abs(this.montoMovimiento) - suma;
            }
        }

        /// <summary>
        /// Indica TRUE si el monto del abono ya ha sido distribuido completamente en un conjunto de prestamos o si
        /// la instancia de prestamo ya ha sido saldada por un conjunto de abonos. En caso contrario, arroja FALSE.
        /// </summary>
        public bool agotado { get { return this.montoActivo <= 0; } }

        internal int liberarPrestamo(ApplicationDbContext db)
        {
            var prestamoAbonos = db.Prestamo_Abono.Where(mov => mov.prestamoID == this.idMovimiento);
            db.Prestamo_Abono.RemoveRange(prestamoAbonos);
            return db.SaveChanges();
        }

        /// <summary>
        /// Determina el tipo de movimiento segun la clase del movimiento
        /// </summary>
        /// <returns>El enum correspondiente al tipo.</returns>
        public TypeOfMovements getTypeOfMovement()
        {
            if (this is PagoPorProducto)
                return TypeOfMovements.PAGO_POR_PRODUCTO;
            else if (this is PrestamoYAbonoCapital)
                return TypeOfMovements.CAPITAL;
            else if (this is VentaACredito)
                return TypeOfMovements.VENTA_A_CREDITO;
            else if (this is LiquidacionSemanal)
                return TypeOfMovements.LIQUIDACION;
            else if (this is Retencion)
                return TypeOfMovements.RENTENCION;
            else
                return TypeOfMovements.NONE;
        }

        public string getNombreControlador()
        {
            TypeOfMovements tom = this.getTypeOfMovement();
            if (tom == TypeOfMovements.CAPITAL)
                return "PrestamoYAbonoCapitals";
            else if (tom == TypeOfMovements.PAGO_POR_PRODUCTO)
                return "PagosPorProductos";
            else if (tom == TypeOfMovements.VENTA_A_CREDITO)
                return "VentaACreditos";
            else if (tom == TypeOfMovements.LIQUIDACION)
                return "LiquidacionSemanal";
            else
                return "";
        }

        /// <summary>
        /// Arroja verdadero si el movimiento es una venta a credito de arbol de olivo.
        /// </summary>
        /// <returns></returns>
        public bool isVentaDeOlivo()
        {
            bool hayOlivo = false; bool res = false;

            //Se valida si es una venta de arboles de olivo
            bool tom = this.getTypeOfMovement() == TypeOfMovements.VENTA_A_CREDITO;
            if (tom)
            {
                VentaACredito ven = ((VentaACredito)this);
                //Si al menos un arbol de olivo se ha vendido, la venta se marca y 
                //se acaba la busqueda
                hayOlivo = VentaACredito.isVentaOlivo(ven.ComprasProductos);
            }
            res = tom && hayOlivo;

            //O si es un abono al balance de olivo
            if (!res)
                res = res || (this.getTypeOfMovement() == TypeOfMovements.CAPITAL &&
                    ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO_ARBOLES);

            return res;
        }

        /// <summary>
        /// Arroja verdadero si el movimiento es un prestamo u abono (movimientos de capital) o 
        /// una venta a credito (exceptuando ventas de arbol de olivo).
        /// </summary>
        /// <returns></returns>
        public bool isAbonoOPrestamo()
        {
            var tom = this.getTypeOfMovement();
            return (tom == (TypeOfMovements.CAPITAL) 
                && ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital 
                    != PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO_ARBOLES) 
                || (tom == TypeOfMovements.VENTA_A_CREDITO && !this.isVentaDeOlivo());
        }

        /// <summary>
        /// Arroja verdadero si es una emision de cheque, una retencion o un pago por producto
        /// </summary>
        /// <returns></returns>
        public bool isMovimientoDeLiquidacion()
        {
            var tom = this.getTypeOfMovement();
            return tom == TypeOfMovements.LIQUIDACION || tom == TypeOfMovements.RENTENCION || tom == TypeOfMovements.PAGO_POR_PRODUCTO;
        }

        /// <summary>
        /// Ajuste general de movimiento, donde se suma a la fecha del movimiento la hora exacta en el que dio de alta el movimiento
        /// para proposito de conferir una fecha unica a cada movimiento que permita el cronologico apropiado de los movimientos
        /// para permitir calcular los balances.
        /// </summary>
        public void ajustarMovimiento(DateTime? originalDate = null)
        {
            //Si el registro es nuevo, se agrega la hora de registro a la fecha del movimiento solo para
            //diferencia movimientos hecho el mismo dia
            if (!originalDate.HasValue) { 
                this.fechaMovimiento = this.fechaMovimiento
                    .AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute)
                    .AddSeconds(DateTime.Now.Second);
            }else //Si el registro ya existia
            {
                //Se ajusta el movimiento mantiendo la hora de su registro aunque el dia haya sido modificado
                this.fechaMovimiento = this.fechaMovimiento
                    .AddHours(originalDate.Value.Hour).AddMinutes(originalDate.Value.Minute)
                    .AddSeconds(originalDate.Value.Second);
            }
        }

        /// <summary>
        /// Introduce la fecha del movimiento dentro del año de periodo de cosecha indicado, tomando por defecto
        /// el día y mes de la fecha de registro.
        /// </summary>
        /// <param name="anioCosecha">Año que representa un periodo de cosecha</param>
        internal void introducirMovimientoEnPeriodo(int? periodoID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            introducirMovimientoEnPeriodo(periodoID, db);
        }

        /// <summary>
        /// Introduce la fecha del movimiento dentro del año de periodo de cosecha indicado, tomando por defecto
        /// el día y mes de la fecha de registro.
        /// </summary>
        /// <param name="anioCosecha">Año que representa un periodo de cosecha</param>
        /// <param name="db">Instancia representativa de la base de datos</param>
        public void introducirMovimientoEnPeriodo(int? periodoID, ApplicationDbContext db)
        {
            TemporadaDeCosecha tem = TemporadaDeCosecha.findTemporada(periodoID);
            introducirMovimientoEnPeriodo(tem);
        }

        /// <summary>
        /// Configura las fechas y variables de temporada de la instanca del movimiento para corresponder
        /// a la instancia de temporada introducida como argumento.
        /// </summary>
        /// <param name="tem">Instancia de temporada dentro de la cual se introduce el movimiento.</param>
        public void introducirMovimientoEnPeriodo(TemporadaDeCosecha tem)
        {
            this.temporadaDeCosecha = tem;
            this.TemporadaDeCosechaID = tem.TemporadaDeCosechaID;
            int anioCosecha = tem.fechaInicio.Year;
            //Si la fecha determinada no entra dentro del periodo de cosecha
            this.fechaMovimiento = new DateTime(anioCosecha, DateTime.Now.Month, DateTime.Now.Day);
            //Si es mayor al rango de periodo de cosecha
            if (this.fechaMovimiento > tem.fechaFin)
                this.fechaMovimiento = tem.fechaFin; //Se le resta un año
            else if (this.fechaMovimiento < tem.fechaInicio)
                this.fechaMovimiento = tem.fechaInicio; //Se le suma un año

            //Se establece como fecha a pagar el 15 de agosto mas próximo
            //TODO: Panel de control de configuraciones generales deberá permitir la modificacion de esta fecha
            if(this.isAbonoOPrestamo() && !this.isAbonoCapital) { 
                ((PrestamoYAbonoCapital)this).fechaPagar = 
                    new DateTime(((PrestamoYAbonoCapital)this).fechaMovimiento.Year, 8, 15);
                if (((PrestamoYAbonoCapital)this).fechaMovimiento > ((PrestamoYAbonoCapital)this).fechaPagar)
                    ((PrestamoYAbonoCapital)this).fechaPagar = ((PrestamoYAbonoCapital)this).fechaPagar.Value.AddYears(1);
            }
        }

        private int DIA_INICIO_PERIODO = 30;
        private int MES_PERIODO = 8;
        public int anioCosecha
        {
            get
          {
               int anioCosecha = this.fechaMovimiento.Year;
               if (this.fechaMovimiento > new DateTime(this.fechaMovimiento.Year,
                      this.MES_PERIODO, this.DIA_INICIO_PERIODO))
                  anioCosecha++;
               return anioCosecha;
            }
        }

        public List<VMInteres> generarSeguimientoPagosConInteres(DateTime fechaActual)
        {
            //Para ventas y prestamos dentro del balance de anticipos
            if(this.tipoDeBalance == TipoDeBalance.CAPITAL_VENTAS && !this.isAbonoCapital) { 
                //Se determina la cantidad de meses entre la fecha original del movimiento la fecha de consulta
                int cantMeses = ((fechaActual.Year - this.fechaMovimiento.Year) * 12) + fechaActual.Month - this.fechaMovimiento.Month;
                cantMeses = cantMeses < 0 ? 0 : cantMeses;

                //Se genera una tabla vacia de seguimiento de pagos de interes
                List<VMInteres> interesesTemp = new List<VMInteres>();
                for (int c = 1; c <= cantMeses; c++)
                    interesesTemp.Add(new VMInteres { numMes = c });

                //Se genera una lista encadenada para rellenar la informacion segun los pagos recibidos
                LinkedList<VMInteres> intereses = new LinkedList<VMInteres>(interesesTemp);
                var interesReg = intereses.First;
                while (interesReg != null)
                {
                    //Es el primero, se basa sobre el monto prestado
                    if (interesReg.Previous == null) {
                        VMInteres inicial = new VMInteres()
                        {numMes = 0,saldoCapital = this.montoMovimiento,balance = this.montoMovimiento};
                        interesReg.Value.calcular(inicial);
                    }
                    else //No es el primero, se basa sobre el saldo capital del mes anterior
                        interesReg.Value.calcular(interesReg.Previous.Value);

                    //Siguiente mes
                    interesReg = interesReg.Next;
                }
                return intereses.ToList();
            }
            return null;
        }

        public class VMInteres
        {
            const decimal INTERES_ANUAL = (decimal).10, INTERES_MENSUAL = INTERES_ANUAL / 12;
            
            [DisplayName("Mes")]
            public int numMes { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Interes")]
            public decimal interes { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Interes Acumulado")]
            public decimal interesAcum { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Deuda al mes")]
            public decimal deuda { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Pagado en el mes")]
            public decimal pago { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Interes Restante en el Mes")]
            public decimal interesRestante { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Saldo Capital")]
            public decimal saldoCapital { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}")]
            [DisplayName("Balance del mes")]
            public decimal balance { get; set; }

            internal void calcular(VMInteres mesAnterior, decimal pago=0)
            {
                this.interes = mesAnterior.saldoCapital * INTERES_MENSUAL;
                this.interesAcum = this.interes + mesAnterior.interesRestante;
                this.deuda = this.interes + mesAnterior.balance;
                this.pago = pago;
                this.interesRestante = this.pago > this.interesRestante ? 0 : this.interesAcum - this.pago;
                this.saldoCapital = mesAnterior.saldoCapital - (this.pago >= this.interesAcum ? this.pago - this.interesAcum : 0);
                this.balance = this.interesRestante + this.saldoCapital;
            }
        }
    }
}