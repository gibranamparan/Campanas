using CampanasDelDesierto_v1.HerramientasGenerales;
using CampanasDelDesierto_v1.Models.SistemaProductores;
using System;
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
        [Display(Name = "Balance Capital (USD)")]
        public decimal balance { get; set; }
        

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Balance (USD)")]
        public decimal balanceMasInteres { get {
                decimal interes = 0;
                decimal interesTotalGenerado = 0;
                if (this.tipoDeBalance == TipoDeBalance.CAPITAL_VENTAS && !this.isAbonoCapital)
                    interes = -this.getInteresRestante(DateTime.Today, out interesTotalGenerado);
                return this.balance + interes;
            } }

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

        public decimal abonoTotalAInteres
        {
            get
            {
                decimal res = 0;

                if (this.abonosRecibidos != null && this.abonosRecibidos.Count() > 0)
                    res = this.abonosRecibidos.Where(abo => abo.pagoAInteres).Sum(abo => abo.monto);

                return res;
            }
        }

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
                else if (this.getTypeOfMovement() == TypeOfMovements.ADEUDO_INICIAL)
                    res = ((AdeudoInicial)this).concepto;

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
                else if (tom == TypeOfMovements.RENTENCION)
                {
                    return "RETENCION";
                }
                else if (tom == TypeOfMovements.ADEUDO_INICIAL)
                {
                    return "ADEUDO INICIAL";
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Regresa el primer movimiento asociado dentro de su coleccion de abonos recibidos
        /// o prestamos abonados (segun si es abono o prestamo), en caso de no existir, regresa
        /// por defecto a si mismo.
        /// </summary>
        /// <returns></returns>
        public MovimientoFinanciero primerMovimientoAsociado()
        {
            MovimientoFinanciero res = this;
            List<Prestamo_Abono> movs = new List<Prestamo_Abono>();
            if(this.tipoDeBalance == TipoDeBalance.CAPITAL_VENTAS)
            {
                if (this.isAbonoCapital && ((PrestamoYAbonoCapital)this).prestamosAbonados != null
                        && ((PrestamoYAbonoCapital)this).prestamosAbonados.Count() > 0)
                {
                    movs = ((PrestamoYAbonoCapital)this).prestamosAbonados
                        .OrderBy(mov => mov.prestamo.fechaMovimiento)
                        .ToList();
                    res = (MovimientoFinanciero)(movs.FirstOrDefault() == null ?
                        this : movs.FirstOrDefault().prestamo);
                }
                else if(this.abonosRecibidos != null && this.abonosRecibidos.Count() > 0)
                {
                    movs = this.abonosRecibidos
                        .OrderBy(mov => mov.abono.fechaMovimiento)
                        .ToList();

                    res = (MovimientoFinanciero)(movs.FirstOrDefault() == null ?
                        this : movs.FirstOrDefault().abono);
                }
            }

            return res;
        }

        /// <summary>
        /// Detecta que la instancia es un movimiento de abono retenido, pero no asociado a ninguna liquidacion.
        /// </summary>
        /// <param name="db">Contexto de la base de datos sobre la cual se buscara la liquidacion asociada.</param>
        /// <returns></returns>
        internal bool isAbonoRetencionSinLiquidacion(ApplicationDbContext db)
        {
            bool res = false;

            if (this.isAbonoRetenidoEnLiquidacion)
            {
                var liq = db.LiquidacionesSemanales.Find(((PrestamoYAbonoCapital)this).abonoEnLiquidacionID);
                if (liq == null)
                    res = true;
            }

            return res;
        }

        /// <summary>
        /// Enumeracion de tipos de balances bajo los que se que agrupan los diferentes movimientos financieros.
        /// </summary>
        public enum TipoDeBalance
        {
            NONE, CAPITAL_VENTAS, VENTA_OLIVO, MOV_LIQUIDACION
        }

        public static string getNombreDeTipoBalance(TipoDeBalance t)
        {
            string res = string.Empty;
            if (t == TipoDeBalance.CAPITAL_VENTAS)
                res = "Anticipos de capital y ventas de material";
            if (t == TipoDeBalance.VENTA_OLIVO)
                res = "Ventas de arboles de olivo";
            if (t == TipoDeBalance.MOV_LIQUIDACION)
                res = "Ingresos de cosecha y liquidaciones";
            return res;
        }

        /// <summary>
        /// Arroja el tipo de balance correspondiente a esta instancia segun su tipo de movimiento y
        /// características particulares. Ver los metodos isVentaDeOlivo(), isAbonoOPrestamo() y 
        /// isMovimientoDeLiquidacion().
        /// </summary>
        public TipoDeBalance tipoDeBalance { get
            {
                //if (this.isAbonoOPrestamo() || this.isAdeudoInicialAnticiposCapital)
                if (this.isAbonoOPrestamo())
                {
                    return TipoDeBalance.CAPITAL_VENTAS;
                } else if (this.isMovimientoDeLiquidacion())
                {
                    return TipoDeBalance.MOV_LIQUIDACION;
                } else if (this.isBalanceDeVentaDeOlivo())
                {
                    return TipoDeBalance.VENTA_OLIVO;
                }else if(this.getTypeOfMovement() == TypeOfMovements.ADEUDO_INICIAL)
                {
                    return ((AdeudoInicial)this).balanceAdeudado;
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
            RENTENCION,
            ADEUDO_INICIAL
        };

        public MovimientoFinanciero() { }

        /// <summary>
        /// Determina el total liquidado del saldo capital de un prestamo. De todos los abonos hechos a este movimiento, se
        /// suma  solamente aquellos no hechos a intereses.
        /// </summary>
        public decimal totalLiquidadoPrestamo
        {
            get
            {
                return this.abonosRecibidos != null ? 
                    this.abonosRecibidos.Where(mov=>!mov.pagoAInteres).Sum(mov => mov.monto) : 0;
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
                var tom = this.getTypeOfMovement();
                if ( tom == TypeOfMovements.CAPITAL)
                {
                    suma = ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO
                    ? this.totalGastadoAbono :
                    ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO ?
                    this.totalLiquidadoPrestamo : 0;
                }
                else if (tom == TypeOfMovements.VENTA_A_CREDITO || tom == TypeOfMovements.ADEUDO_INICIAL)
                {
                    suma = this.totalLiquidadoPrestamo;
                }

                return Math.Abs(this.montoMovimiento) - suma;
            }
        }

        public string montoALetra { get {
                return NumbersTools.convertirMontoALetra(this.montoMovimiento, "Dólares")+ " U.S.Cy.";
            } }

        /// <summary>
        /// Indica TRUE si el monto del abono ya ha sido distribuido completamente en un conjunto de prestamos o si
        /// la instancia de prestamo ya ha sido saldada por un conjunto de abonos. En caso contrario, arroja FALSE.
        /// </summary>
        public bool agotado { get { return Math.Round(this.montoActivo,2) <= 0; } }

        public static System.Data.Entity.EntityState determinarEstadoMovimiento(MovimientoFinanciero adeudo)
        {
            bool hayMonto = Math.Abs(adeudo.montoMovimiento) > 0, existe = adeudo.idMovimiento > 0;
            if (hayMonto && existe)
                return System.Data.Entity.EntityState.Modified;
            else if (hayMonto && !existe)
                return System.Data.Entity.EntityState.Added;
            else if (!hayMonto && existe)
                return System.Data.Entity.EntityState.Deleted;
            else
                return System.Data.Entity.EntityState.Detached;
        }

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
            else if (this is AdeudoInicial)
                return TypeOfMovements.ADEUDO_INICIAL;
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
            else if (tom == TypeOfMovements.ADEUDO_INICIAL)
                return "AdeudoInicial";
            else if (tom == TypeOfMovements.RENTENCION)
                return "Retencion";
            else
                return "";
        }

        /// <summary>
        /// Arroja verdadero si el movimiento es una venta a credito de arbol de olivo.
        /// </summary>
        /// <returns></returns>
        public bool isBalanceDeVentaDeOlivo()
        {
            bool res = this.isVentaDeArbolOlivo;

            //O si es un abono al balance de olivo
            if (!res)
                res = res || (this.getTypeOfMovement() == TypeOfMovements.CAPITAL &&
                    ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO_ARBOLES);

            return res;
        }

        public bool isVentaDeArbolOlivo { get {
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
            return res;
        } }

        /// <summary>
        /// Arroja verdadero si el movimiento es un prestamo u abono (movimientos de capital) o 
        /// una venta a credito (exceptuando ventas de arbol de olivo).
        /// </summary>
        /// <returns></returns>
        public bool isAbonoOPrestamo()
        {
            var tom = this.getTypeOfMovement();
            return (tom == (TypeOfMovements.CAPITAL)  //Si es un movimiento de capital, no abono de arboles
                && ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital 
                    != PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO_ARBOLES) 
                || (tom == TypeOfMovements.VENTA_A_CREDITO && !this.isBalanceDeVentaDeOlivo());
        }

        /// <summary>
        /// Arroja verdadero si es un movimiento de anticipo de capital.
        /// </summary>
        public bool isAnticipoDeCapital
        {
            get {
                return this.tipoDeBalance == TipoDeBalance.CAPITAL_VENTAS && !this.isAbonoCapital &&
                       this.getTypeOfMovement() != TypeOfMovements.VENTA_A_CREDITO && this.isAbonoOPrestamo();
            }
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

            //La fecha por defecto de los anticipos tendra como tope el inicio o fin de la temporada
            int anioCosecha = this.fechaMovimiento.Year;
            if (anioCosecha > tem.fechaFin.Year)
                anioCosecha = tem.fechaFin.Year;
            else if (anioCosecha < tem.fechaInicio.Year)
                anioCosecha = tem.fechaInicio.Year;

            //Si la fecha determinada no entra dentro del periodo de cosecha
            this.fechaMovimiento = new DateTime(anioCosecha, DateTime.Now.Month, DateTime.Now.Day);
            //Si es mayor al rango de periodo de cosecha
            if (this.fechaMovimiento > tem.fechaFin)
                this.fechaMovimiento = tem.fechaFin; //Se le resta un año
            else if (this.fechaMovimiento < tem.fechaInicio)
                this.fechaMovimiento = tem.fechaInicio; //Se le suma un año

            //Se establece como fecha a pagar el 15 de agosto mas próximo para anticipos de capital
            if (this.getTypeOfMovement() == TypeOfMovements.CAPITAL && !this.isAbonoCapital)
            {
                ((PrestamoYAbonoCapital)this).fechaPagar =
                    new DateTime(((PrestamoYAbonoCapital)this).fechaMovimiento.Year, this.MES_PERIODO, this.DIA_PAGO);
                if (((PrestamoYAbonoCapital)this).fechaMovimiento > ((PrestamoYAbonoCapital)this).fechaPagar)
                    ((PrestamoYAbonoCapital)this).fechaPagar = ((PrestamoYAbonoCapital)this).fechaPagar.Value.AddYears(1);
            }

            //Se establece como fecha a pagar el 15 de agosto mas próximo para ventas a credito
            if (this.getTypeOfMovement() == TypeOfMovements.VENTA_A_CREDITO)
            {
                ((VentaACredito)this).fechaPagar =
                    new DateTime(((VentaACredito)this).fechaMovimiento.Year, this.MES_PERIODO, this.DIA_PAGO);
                if (((VentaACredito)this).fechaMovimiento > ((VentaACredito)this).fechaPagar)
                    ((VentaACredito)this).fechaPagar = ((VentaACredito)this).fechaPagar.Value.AddYears(1);
            }
        }

        private int DIA_INICIO_PERIODO = 30;
        private int MES_PERIODO = 8;
        private int DIA_PAGO = 15;

        public bool isAbonoRetenidoEnLiquidacion { get
            {
                bool res = this.getTypeOfMovement() == TypeOfMovements.CAPITAL && ((PrestamoYAbonoCapital)this).abonoEnLiquidacionID.HasValue 
                    && ((PrestamoYAbonoCapital)this).abonoEnLiquidacionID.Value > 0;

                return res;
            }
        }

        /// <summary>
        /// Es verdadero si es un adeudo inicial para ventas de materiales
        /// </summary>
        public bool isAdeudoInicialMaterial
        {
            get
            {
                return this.getTypeOfMovement() == TypeOfMovements.ADEUDO_INICIAL
                    && ((AdeudoInicial)this).isVentas.HasValue && ((AdeudoInicial)this).isVentas.Value &&
                    this.tipoDeBalance != TipoDeBalance.VENTA_OLIVO;
            }
        }

        public bool isAdeudoInicialAnticiposCapital
        {
            get
            {
                bool isAdeudoInicial = this.getTypeOfMovement() == TypeOfMovements.ADEUDO_INICIAL;
                return isAdeudoInicial && !((((AdeudoInicial)this).isVentas.HasValue && ((AdeudoInicial)this).isVentas.Value) || 
                    this.tipoDeBalance == TipoDeBalance.VENTA_OLIVO);
            }
        }

        public bool isAdeudoInicialVentaOlivo
        {
            get
            {
                return this.getTypeOfMovement() == TypeOfMovements.ADEUDO_INICIAL
                    && this.tipoDeBalance == TipoDeBalance.VENTA_OLIVO
                    && !(((AdeudoInicial)this).isVentas.HasValue && ((AdeudoInicial)this).isVentas.Value);
            }
        }

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

        public bool isAdeudoInicial { get {
                return isAdeudoInicialAnticiposCapital || isAdeudoInicialMaterial || isAdeudoInicialVentaOlivo;
            } }

        public bool isNoDirectamenteModificable
        {
            get
            {
                return this.isAbonoRetenidoEnLiquidacion || this.isAdeudoInicial;
            }
        }

        public bool isVentaDeMaterial { get {
                return this.getTypeOfMovement() == TypeOfMovements.VENTA_A_CREDITO && this.tipoDeBalance == TipoDeBalance.CAPITAL_VENTAS;
            } }

        public List<VMInteres> generarSeguimientoPagosConInteres(DateTime fechaActual)
        {
            //Para vprestamos dentro del balance de anticipos
            if(this.isAnticipoDeCapital || this.isAdeudoInicialAnticiposCapital) { 
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
                    //decimal totalAbonos = Math.Round(this.getTotalAbonosDelMes(interesReg.Value.numMes),2);
                    decimal totalAbonos = this.getTotalAbonosDelMes(interesReg.Value.numMes);
                    //Es el primero, se basa sobre el monto prestado
                    if (interesReg.Previous == null) {
                        //Se toman los abonos recibidos por el movimiento
                        List<Prestamo_Abono> abonosAnteriores = new List<Prestamo_Abono>();
                        if (this.abonosRecibidos != null && this.abonosRecibidos.Count > 0) {
                            DateTime diaUltimoMes = new DateTime(this.fechaMovimiento.Year, this.fechaMovimiento.Month, 
                                DateTime.DaysInMonth(this.fechaMovimiento.Year, this.fechaMovimiento.Month)).AddDays(1);
                            //Se toman los abonos que podria haber recibido este movimiento antes de haberse registrado 
                            //o antes de comenzar a generar interes
                            abonosAnteriores = this.abonosRecibidos
                                //.Where(mov => mov.abono.fechaMovimiento <= this.fechaMovimiento)
                                .Where(mov => mov.abono.fechaMovimiento < diaUltimoMes)
                                .Where(mov => !mov.pagoAInteres).ToList();
                        }

                        //Si este mes recibio abonos a capital dentro del mes de su origen o antes
                        decimal montoInicial;
                        if (abonosAnteriores.Count() > 0) {
                            var sumaDeAbonosAnteriores = abonosAnteriores.Sum(mov => mov.monto);
                            montoInicial = Math.Abs(this.montoMovimiento) - sumaDeAbonosAnteriores;
                        }
                        else
                            montoInicial = Math.Abs(this.montoMovimiento);

                        //En caso de ser adeudo inicial, se debe incluir el interes inicial ya adeudado por el productor
                        //al ser ingresado al sistema
                        decimal interesRestante = 0;
                        if (this.getTypeOfMovement() == TypeOfMovements.ADEUDO_INICIAL)
                            interesRestante = ((AdeudoInicial)this).interesInicial;

                        //Se calcula un registro tomando los parametros iniciales del prestamo, sobre este se calculara cada interes por mes de forma consecutiva
                        VMInteres inicial = new VMInteres(){numMes = 0,saldoCapital = Math.Abs(montoInicial),
                            balance = Math.Abs(montoInicial)+ interesRestante, interesRestante = interesRestante};

                        interesReg.Value.calcular(inicial, totalAbonos);
                    }
                    else //No es el primero, se basa sobre el saldo capital del mes anterior
                        interesReg.Value.calcular(interesReg.Previous.Value, totalAbonos);

                    //Si uno de los registros de intereses da la deuda en 0, no es necesario seguir iterando
                    if (Math.Round(interesReg.Value.deuda, 2) == 0)
                        break;

                    //Siguiente mes
                    interesReg = interesReg.Next;
                }
                var res = intereses.ToList();
                res.RemoveAll(mov => Math.Round(mov.deuda,2) == 0);
                return res;
            }
            return null;
        }

        /// <summary>
        /// Determina el total de abonos depositados en un mes para la deuda representada por la instancia actual
        /// dado un numero entero que representa una numeración ordenada de los meses ne los que la deuda esta activa.
        /// </summary>
        /// <param name="numMes">Numero indice para abarcar la totalidad del mes indexado.</param>
        /// <returns></returns>
        public decimal getTotalAbonosDelMes(int numMes)
        {
            //Se interpreta el indice en un mes especifico a partir del origen de la deuda.
            DateTime dt = this.fechaMovimiento.AddMonths(numMes);

            //Se forma el rango de tiempo a partir del indice sobre el que se calculara el abono total recibido
            DateTime startDate = new DateTime(dt.Year, dt.Month, 1);
            DateTime endDate = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
            endDate = endDate.AddDays(1).AddMilliseconds(-1);
            TimePeriod tp = new TimePeriod(startDate, endDate);

            return getTotalAbonosDelMes(tp);
        }

        /// <summary>
        /// Dada una fecha 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private decimal getTotalAbonosDelMes(TimePeriod tp)
        {
            decimal totalAbonos = 0;

            //Se calcula el total si hay abonos registrados.
            if (this.abonosRecibidos != null && this.abonosRecibidos.Count() > 0)
            {
                totalAbonos = this.abonosRecibidos.Where(mov => tp.hasInside(mov.abono.fechaMovimiento))
                    .Sum(mov => mov.monto);
            }
            return totalAbonos;
        }

        /// <summary>
        /// Arroja el interes por pagar y total generado acumulado a la fecha actual de la deuda en cuestion (balance de anticipos).
        /// </summary>
        /// <param name="fechaActual">Fecha de consulta de los intereses</param>
        /// <param name="interesTotalGenerado">Argumento de salida sobre el cual se arroja el total de intereses generados por este anticipo.</param>
        /// <returns></returns>
        public decimal getInteresRestante(DateTime fechaActual, out decimal interesTotalGenerado)
        {
            interesTotalGenerado = 0;
            if (this.isAnticipoDeCapital || this.isAdeudoInicialAnticiposCapital)
            {
                List<VMInteres> intereses = this.generarSeguimientoPagosConInteres(fechaActual);
                var interesDevengado = (intereses != null && intereses.Count() > 0) ?
                    intereses.Last().interesRestante : 0;
                interesTotalGenerado = intereses.Sum(reg => reg.interes);

                interesDevengado = Math.Round(interesDevengado, 2);
                //interesTotalGenerado = Math.Round(interesTotalGenerado, 2);

                return interesDevengado;
            }
            return 0;
        }
        
        /// <summary>
         /// Arroja el interes acumulado a la fecha actual de la deuda en cuestion, dentro del balance de anticipos.
         /// </summary>
         /// <param name="fechaActual"></param>
         /// <returns></returns>
        public decimal getInteresRestante(DateTime fechaActual)
        {
            if (this.isAnticipoDeCapital || this.isAdeudoInicialAnticiposCapital)
            {
                var intereses = this.generarSeguimientoPagosConInteres(fechaActual);
                var interesDevengado = (intereses != null && intereses.Count() > 0) ?
                    intereses.Last().interesRestante : 0;
                return interesDevengado;
            }
            return 0;
        }

        /// <summary>
        /// Clase que guarda calcula de manera simple el total de deudas, deudas iniciales, abonos y un balance de las mismas, basado en ua lista de movimientos dada
        /// al constructor.
        /// </summary>
        public class VMTotalesSimple
        {
            /// <summary>
            /// Total de abonos introducidos dentro de la temporada actual
            /// </summary>
            public decimal abonos { get; set; }
            /// <summary>
            /// Total de deuda introducida en la temporada actual
            /// </summary>
            public decimal deudas { get; set; }
            /// <summary>
            /// Monto total de la deuda debida desde la temporada anterior
            /// </summary>
            public decimal deudaInicial { get; private set; }
            /// <summary>
            /// Suma de la deuda inicial con la deuda total de la cosecha actual
            /// </summary>
            public decimal deudaTotal { get; set; }
            /// <summary>
            /// Balance de la temporada actual
            /// </summary>
            public decimal balance { get; set; }

            public VMTotalesSimple() { }
            /// <summary>
            /// Calcula el total de deudaInicial, abonos, deudas, deudaTotal y balance
            /// </summary>
            /// <param name="lista">Lista sobre la cual se calcularan los totales</param>
            public VMTotalesSimple(List<MovimientoFinanciero> lista)
            {
                this.deudaInicial = lista.Where(mov => mov.isAdeudoInicialVentaOlivo).Sum(mov => mov.montoMovimiento);
                this.abonos = lista.Where(mov=>mov.montoMovimiento>0).Sum(mov => mov.montoMovimiento);
                this.deudas = lista.Where(mov => mov.montoMovimiento<0  && !mov.isAdeudoInicialVentaOlivo).Sum(mov => mov.montoMovimiento);
                this.deudaTotal = this.deudas + this.deudaInicial;
                this.balance = this.abonos + this.deudaTotal;
            }
        }

        /// <summary>
        /// Arroja una instancia VMInteres que contiene informacion sobre el estado de los intereses para el movimiento en cuestion
        /// generado hasta la fecha indicada.
        /// </summary>
        /// <param name="fechaActual">Fecha hasta la cual se calcula el registro de intereses para la presente instancia.</param>
        /// <param name="interesTotalGenerado">Argumento de salida donde se muestra el interes total generado durante todo el historial de intereses de la instancia de movimiento.</param>
        /// <returns>Arroja una instancia VMInteres con la informacion hasta la fecha indicada de los intereses del movimiento.</returns>
        public VMInteres getInteresReg(DateTime fechaActual, out decimal interesTotalGenerado)
        {
            interesTotalGenerado = 0;
            if (this.tipoDeBalance == TipoDeBalance.CAPITAL_VENTAS && !this.isAbonoCapital)
            {
                var intereses = this.generarSeguimientoPagosConInteres(fechaActual);
                interesTotalGenerado = intereses.Sum(reg => reg.interes);
                return intereses.LastOrDefault()==null?new VMInteres():intereses.LastOrDefault();
            }
            return new VMInteres();
        }

        public override string ToString()
        {
            return String.Format("Fecha: {0}, Monto: {1}, Tipo: {2}", 
                this.fechaMovimiento,this.montoMovimiento, this.nombreDeMovimiento);
        }

        public class VMInteres
        {
            const decimal INTERES_ANUAL = .10M, INTERES_MENSUAL = INTERES_ANUAL / 12;
            
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
                this.interesRestante = this.pago > this.interesAcum ? 0 : this.interesAcum - this.pago;
                this.saldoCapital = mesAnterior.saldoCapital - (this.pago >= this.interesAcum ? 
                    this.pago - this.interesAcum : 0);
                this.balance = this.interesRestante + this.saldoCapital;

                //this.round();
            }

            public void round()
            {
                this.interes = Math.Round(this.interes,2);
                this.interesAcum = Math.Round(this.interesAcum, 2);
                this.deuda = Math.Round(this.deuda, 2);
                this.interesRestante = Math.Round(this.interesRestante, 2);
                this.saldoCapital = Math.Round(this.saldoCapital, 2);
                this.balance = Math.Round(this.balance, 2);
            }
        }

        public class VMMovimientoBalanceAnticipos {
            public MovimientoFinanciero mov { get; }

            [DisplayName("Fecha")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public DateTime fecha { get; set; }

            [DisplayName("Pagaré")]
            public string pagare { get; set; }

            [DisplayName("Anticipo")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal anticipo { get; set; }

            [DisplayName("Interes")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal interes { get; set; } //total generado (pagado o no)

            [DisplayName("Tipo")]
            public string tipo { get; set; }

            [DisplayName("Concepto")]
            public string concepto { get; set; }

            [DisplayName("Capital")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal abonoCapital { get; set; }

            [DisplayName("Interes")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal abonoInteres { get; set; } //interes pagado

            [DisplayName("Capital")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal saldoCapital { get; set; }

            [DisplayName("Interes")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal saldoInteres { get; set; } //interes restante

            [DisplayName("Total")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal total { get; set; }

            [DisplayName("Balance Capital + Interes")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal balance { get; set; }

            /// <summary>
            /// Recibe la lista de movimientos que se visualizaran en el reporte del balance de anticipos. Rellenando el campo
            /// de totales de cada registro sumando uno tras otro en orden cronologico.
            /// </summary>
            /// <param name="lista">Lista encadenada de VMMovimientoBalanceAnticipos, se asume que esta ordenado en orden cronologico.</param>
            /// <param name="adeudoAnteriorTotal">En caso de haber un adeduo anterior</param>
            public static void balancear(ref LinkedList<VMMovimientoBalanceAnticipos> lista, decimal adeudoAnteriorTotal)
            {
                if (adeudoAnteriorTotal > 0)
                {
                    VMMovimientoBalanceAnticipos adeudoAnterior = new VMMovimientoBalanceAnticipos();
                }

                var nodo = lista.First;
                while (nodo != null)
                {
                    if (!nodo.Value.mov.isAbonoCapital) { 
                        if (nodo.Previous != null)
                        {adeudoAnteriorTotal = nodo.Previous.Value.total;}

                        nodo.Value.total = nodo.Value.saldoCapital + nodo.Value.saldoInteres + adeudoAnteriorTotal;
                    }
                    nodo = nodo.Next;
                }
            }

            public VMMovimientoBalanceAnticipos() { }

            public VMMovimientoBalanceAnticipos(MovimientoFinanciero mov, DateTime fechaConsulta)
            {
                this.mov = mov;
                decimal interesTotalGenerado = 0;
                decimal interesPendiente = 0;
                
                //Se calcula interes para todos los movimientos menos adeudos iniciales que representan el remantente de temporadas anteriores
                if(!(mov.isAdeudoInicialAnticiposCapital && !((AdeudoInicial)mov).isRegistradoInicialmenteEnProductor))
                    interesPendiente = mov.getInteresRestante(fechaConsulta, out interesTotalGenerado);

                //Si es adeudo inicial de anticipos, el interes es la suma del interes inicial mas el interes generado
                interesTotalGenerado += this.mov.isAdeudoInicialAnticiposCapital?((AdeudoInicial)this.mov).interesInicial:0;

                this.fecha = mov.fechaMovimiento;
                this.pagare = mov.isAnticipoDeCapital ? ((PrestamoYAbonoCapital)mov).pagare
                    : mov.isVentaDeMaterial ? ((VentaACredito)mov).pagareVenta : "";
                this.anticipo = mov.montoMovimiento;
                this.interes = interesTotalGenerado;                
                this.tipo = mov.nombreDeMovimiento;
                this.concepto = mov.concepto;

                this.abonoCapital = Math.Abs(this.anticipo) - mov.montoActivo;
                this.abonoInteres = mov.abonoTotalAInteres;

                this.saldoCapital = mov.montoActivo;
                this.saldoInteres = this.interes - this.abonoInteres;

                this.balance = mov.balanceMasInteres;
            }

            public class VMBalanceAnticiposTotales
            {
                /// <summary>
                /// Toda de los montos registados por movimiento incluyendo adeudos iniciales
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal anticipo { get; set; }

                /// <summary>
                /// Total de todos los intereses generados por los anticipos de capital, incluyendo
                /// adeudos iniciales
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal interes { get; set; }

                /// <summary>
                /// Total de todos los intereses generados por los anticipos de capital, excluyendo
                /// adeudos iniciales
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal interesDentroTemporada { get; set; }

                /// <summary>
                /// Total de la distribucion de abonos registrados a capital
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal abonoCapital { get; set; }

                /// <summary>
                /// Total de la distribucion de abonos registrados a interes
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal abonoInteres { get; set; }

                /// <summary>
                /// Total de la cantidad total que se debe actualmente a capital.
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal saldoCapital { get; set; }

                /// <summary>
                /// Total de la cantidad total que se debe actualmente correspondiente sólo a anticipos de capital, 
                /// incluyendo adeudos iniciales por concepto de anticipos de capital.
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal saldoAnticiposCapital { get; set; }

                /// <summary>
                /// Total de la cantidad total que se debe actualmente a interes.
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal saldoInteres { get; set; }

                /// <summary>
                /// Totalidad de montos registrados a ventas a credito.
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal ventasACredito { get; private set; }

                /// <summary>
                /// Totalidad de montos registrados a ventas a credito incluyendo adeudos iniciales
                /// de ventas de materiales.
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal saldoVentasACredito { get; private set; }

                /// <summary>
                /// Suma de los montos correspondientes a los anticipos generados en este periodo solamente.
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal anticiposEfectivo { get; private set; }

                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal deudaCapitalInicial { get; set; }

                /// <summary>
                /// Deuda inicial por interes generado previo a la temporada actual.
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal deudaInteresInicial { get; set; }

                /// <summary>
                /// Deuda inicial por ventas a credito.
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal deudaVentasInicial { get; set; }

                /// <summary>
                /// Deuda inicial por venta a credito de arboles de olivo.
                /// </summary>
                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal deudaVentasArbolesInicial { get; set; }

                public VMBalanceAnticiposTotales() { }
                public VMBalanceAnticiposTotales(IEnumerable<VMMovimientoBalanceAnticipos> lista)
                {
                    //Abonos
                    this.abonoCapital = lista.Where(i => !i.mov.isAbonoCapital).Sum(i => i.abonoCapital);
                    this.abonoInteres = lista.Where(i => !i.mov.isAbonoCapital).Sum(i => i.abonoInteres);

                    //Deudas
                    this.ventasACredito = lista.Where(i => i.mov.isVentaDeMaterial
                    && !i.mov.isAdeudoInicialMaterial).Sum(i => i.anticipo); //Venta de material a credito (no inicial)
                    this.anticiposEfectivo = lista.Where(i => i.mov.isAnticipoDeCapital).Sum(i => i.anticipo);
                    this.interes = lista.Sum(i => i.interes);

                    //Saldo por pagar
                    this.saldoCapital = lista.Where(i => !i.mov.isAbonoCapital).Sum(i => i.saldoCapital);
                    this.saldoInteres = lista.Where(i => !i.mov.isAbonoCapital).Sum(i => i.saldoInteres);
                    this.saldoVentasACredito = lista.Where(i => i.mov.isVentaDeMaterial || i.mov.isAdeudoInicialMaterial)
                        .Sum(i => i.saldoCapital);
                    this.saldoAnticiposCapital = lista.Where(i => i.mov.isAnticipoDeCapital 
                        || i.mov.isAdeudoInicialAnticiposCapital).Sum(i => i.saldoCapital);

                    //Deuda inicial
                    var movDeudaInicial = lista.FirstOrDefault(item => item.mov.isAdeudoInicialAnticiposCapital);
                    this.deudaCapitalInicial = movDeudaInicial == null ? 0 : Math.Abs(movDeudaInicial.anticipo); //Deuda incial por anticipos de capital
                    this.deudaInteresInicial = movDeudaInicial == null ? 0 : Math.Abs(movDeudaInicial.interes); //Deuda incial por intereses de anticipos de capital

                    var movDeudaVentaInicial = lista.FirstOrDefault(item => item.mov.isAdeudoInicialMaterial);
                    this.deudaVentasInicial = movDeudaVentaInicial == null ? 0 : movDeudaVentaInicial.anticipo; //Deuda inicial por ventas de material a credito

                    var movDeudaArbolesInicial = lista.FirstOrDefault(item => item.mov.isAdeudoInicialVentaOlivo);
                    this.deudaVentasArbolesInicial = movDeudaArbolesInicial == null ? 0: movDeudaArbolesInicial.anticipo; //Deuda inicial por compras de arboles de olvio

                    //Total de deuda por anticipos totales
                    this.anticipo = Math.Abs(this.ventasACredito) + Math.Abs(this.anticiposEfectivo)+
                        Math.Abs(this.deudaCapitalInicial) + Math.Abs(this.deudaVentasInicial);
                }
            }
        }

    }
}