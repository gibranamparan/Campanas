﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CampanasDelDesierto_v1.Models;
using System.Web.Mvc;
using OfficeOpenXml;
using CampanasDelDesierto_v1.HerramientasGenerales;
using System.ComponentModel;
using static CampanasDelDesierto_v1.Models.TemporadaDeCosecha;
using Prestamo_Abono = CampanasDelDesierto_v1.Models.PrestamoYAbonoCapital.Prestamo_Abono;
using static CampanasDelDesierto_v1.Models.MovimientoFinanciero;
using CampanasDelDesierto_v1.Models.SistemaProductores;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampanasDelDesierto_v1.Models
{
    public class Productor
    {
        [Key]
        public int idProductor { get; set; }

        [Required]
        [Display(Name = "Número de Productor")]
        public string numProductor { get; set; }

        [Required]
        [Display(Name = "Nombre Productor ")]
        public string nombreProductor { get; set; }

        [Display(Name = "Domicilio")]
        public string domicilio { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de integracion")]
        public DateTime fechaIntegracion { get; set; }

        [Display(Name = "RFC ")]
        public string RFC { get; set; }

        [Display(Name = "Zona")]
        public string zona { get; set; }

        [Display(Name = "Población")]
        public string poblacion { get; set; }

        [Display(Name = "Nombre del Cheque")]
        public string nombreCheque { get; set; }

        [Display(Name = "Representante Legal")]
        public string nombreRepresentanteLegal { get; set; }

        [DisplayName("Teléfono")]
        public string telefono { get; set; }

        [DisplayName("Desactivado")]
        public bool Desactivado { get; set; }
        
        /// <summary>
        /// Coleccion de movimientos financieros registrados al productor.
        /// </summary>
        public virtual ICollection<MovimientoFinanciero> MovimientosFinancieros { get; set; }

        /// <summary>
        /// Conjunto de recibos de ingreso de cosecha que los productores hacen durante el fin de la temporada.
        /// </summary>
        public virtual ICollection<RecepcionDeProducto> recepcionesDeProducto { get; set; }

        /// <summary>
        /// Cheques generados para liberar el pago de rentecion.
        /// </summary>
        [DisplayName("Cheques de Pago de Retención")]
        public virtual ICollection<RetencionCheque> cheques { get; set; }

        public Productor() { }
        /// <summary>
        /// Constructor que crea una copia selectiva de otra instancia del productor
        /// </summary>
        /// <param name="otro"></param>
        public Productor(Productor otro)
        {
            this.numProductor = otro.numProductor;
            this.nombreProductor = otro.nombreProductor;
            this.zona = otro.zona;
            this.domicilio = otro.domicilio;
            this.RFC = otro.RFC;
            this.nombreCheque = otro.nombreCheque;
            this.nombreRepresentanteLegal = otro.nombreRepresentanteLegal;
            this.fechaIntegracion = otro.fechaIntegracion;
        }

        /// <summary>
        /// Genera una instancia de productor basado del renglon provenienten de un excel.
        /// </summary>
        /// <param name="rowProductor">RangeExcel proveniente de un excel cargado.</param>
        /// <param name="error">Instancia sobre la cual se capturara un posible error al cargar el renglon en la nueva instancia.</param>
        public Productor(ExcelRange rowProductor, ref ExcelTools.ExcelParseError error)
        {
            error = new ExcelTools.ExcelParseError();
            try
            {
                this.numProductor = rowProductor.ElementAt((int)ExcelColumns.NUM).Value.ToString().Trim();
                this.nombreProductor = rowProductor.ElementAt((int)ExcelColumns.NOMBRE).Value.ToString();
                this.domicilio = rowProductor.ElementAt((int)ExcelColumns.DIRECCION).Value.ToString();
                this.RFC = rowProductor.ElementAt((int)ExcelColumns.RFC).Value.ToString();
                this.zona = rowProductor.ElementAt((int)ExcelColumns.ZONA).Value.ToString();
                this.zona = this.zona.Replace("Zona", "").Trim();
                this.nombreCheque = rowProductor.ElementAt((int)ExcelColumns.NOMBRE_DEL_CHEQUE).Value.ToString();

                this.fechaIntegracion = DateTime.Today;
            }
            catch (NullReferenceException exc)
            {
                error = ExcelTools.ExcelParseError.errorFromException(exc, rowProductor);
                error.registro = new Productor(this);
            }
            catch (Exception exc)
            {
                error = ExcelTools.ExcelParseError.errorFromException(exc, rowProductor);
                error.registro = new Productor(this);
            }
        }

        

        public decimal totalDeudaBalanceAnticiposPorTemporada(int temporadaID)
        {
            decimal res = 0;
            if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count()>0) {
                var movimientos = this.MovimientosFinancieros
                    .Where(mov => mov.tipoDeBalance == TipoDeBalance.CAPITAL_VENTAS && !mov.isAbonoCapital)
                    .Where(mov=>mov.TemporadaDeCosechaID == temporadaID);
                res = Math.Abs(movimientos.Sum(mon => mon.montoMovimiento));
            }
            return res;
        }

        public decimal totalAbonosBalanceAnticiposPorTemporada(int temporadaID)
        {
            decimal res = 0;
            if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0)
            {
                res = this.MovimientosFinancieros.Where(mov => mov.getTypeOfMovement() == TypeOfMovements.CAPITAL)
                    .Where(mov => ((PrestamoYAbonoCapital)mov).tipoDeMovimientoDeCapital
                        == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO)
                        .Where(mov=>mov.TemporadaDeCosechaID == temporadaID)
                        .Sum(mov => mov.montoMovimiento);
            }
            return res;
        }

        /// <summary>
        /// Calcula el monto total de arboles de olivo comprados dentro de una temporada indicada
        /// </summary>
        /// <param name="temporadaID"></param>
        /// <returns></returns>
        public decimal totalDeudaVentaArbolitoPorTemporada(int temporadaID)
        {
            decimal res = 0;
            if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0) {
                var movimientos = this.MovimientosFinancieros
                    .Where(mov => mov.TemporadaDeCosechaID == temporadaID)
                    .Where(mov => mov.isVentaDeArbolOlivo || mov.isAdeudoInicialVentaOlivo);
                res = movimientos.Sum(mov => mov.montoMovimiento);
            }

            return res;
        }

        public decimal totalAbonoArbolitoPorTemporada(int temporadaID)
        {
            decimal res = 0;
            if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0)
            {
                res = this.MovimientosFinancieros.Where(mov=>mov.TemporadaDeCosechaID == temporadaID)
                    .Where(mov => mov.getTypeOfMovement() == TypeOfMovements.CAPITAL)
                    .Where(mov => ((PrestamoYAbonoCapital)mov).tipoDeMovimientoDeCapital
                        == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO_ARBOLES)
                    .Sum(mov => mov.montoMovimiento);
            }
            return res;
        }

        public decimal totalIngresos
        {
            get
            {
                decimal res = 0;
                if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0)
                {
                    res = this.MovimientosFinancieros
                    .Where(mov => mov.getTypeOfMovement() == TypeOfMovements.PAGO_POR_PRODUCTO)
                    .Sum(mon => mon.montoMovimiento);
                }
                return res;
            }
        }

        public decimal totalLiquidacion
        {
            get
            {
                decimal res = 0;
                if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0)
                {
                    res = Math.Abs(this.MovimientosFinancieros
                    .Where(mov=> mov.getTypeOfMovement() == TypeOfMovements.LIQUIDACION)
                    .Sum(mon=>mon.montoMovimiento));
                }
                return res;
            }
        }

        public decimal totalRetenido
        {
            get
            {
                decimal res = 0;
                if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0)
                {
                    res = Math.Abs(this.MovimientosFinancieros
                    .Where(mov => mov.getTypeOfMovement() == TypeOfMovements.RENTENCION)
                    .Sum(mon => mon.montoMovimiento));
                }
                return res;
            }
        }

        public AdeudoInicial adeudoInicialArboles
        {
            get
            {
                var movimientos = this.MovimientosFinancieros;
                if (movimientos != null && movimientos.Count() > 0)
                {
                    var res = movimientos.FirstOrDefault(mov => mov.getTypeOfMovement() == TypeOfMovements.ADEUDO_INICIAL
                    && mov.tipoDeBalance == TipoDeBalance.VENTA_OLIVO);
                    return res != null ? (AdeudoInicial)res : null;
                }
                return null;
            }
        }

        public AdeudoInicial adeudoInicialAnticipos
        {
            get
            {
                var movimientos = this.MovimientosFinancieros;
                if (movimientos != null && movimientos.Count() > 0)
                {
                    var res = movimientos.FirstOrDefault(mov => mov.isAdeudoInicialAnticiposCapital 
                        && mov.tipoDeBalance == TipoDeBalance.CAPITAL_VENTAS);
                    return res != null ? (AdeudoInicial)res : null;
                }
                return null;
            }
        }


        public AdeudoInicial adeudoInicialMateriales
        {
            get
            {
                var movimientos = this.MovimientosFinancieros;
                if (movimientos != null && movimientos.Count() > 0)
                {
                    var res = movimientos.FirstOrDefault(mov => mov.isAdeudoInicialMaterial);
                    return res != null ? (AdeudoInicial)res : null;
                }
                return null;
            }
        }

        [DisplayName("Balance Actual (USD)")]
        public decimal balanceActual
        {
            get
            {
                if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0)
                {
                    var movimientos = this.MovimientosFinancieros
                        .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS).ToList();

                    var lastMov = movimientos.OrderByDescending(mov => mov.fechaMovimiento).FirstOrDefault();
                    decimal balance = lastMov==null?0: lastMov.balance;
                    return balance;
                }
                else return 0;
            }
        }

        /// <summary>
        /// Calcula el monto total de interes pagado por los abonos hechos dentro de la temporada indiquada en el argumento.
        /// </summary>
        /// <param name="tem">Instancia de TemporadaDeCosecha sobre la cual se quiere calcular los intereses pagados.</param>
        /// <returns></returns>
        public decimal getInteresesPagadosEnLaTemporada(TemporadaDeCosecha tem)
        {
            decimal res = 0;
            if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0)
            {
                var movimientos = this.MovimientosFinancieros
                    .Where(mov => mov.TemporadaDeCosechaID <= tem.TemporadaDeCosechaID)
                    .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS
                        && mov.isAbonoCapital).Cast<PrestamoYAbonoCapital>();
                if (movimientos.Count() > 0)
                {
                    //Se almancena dentro de una lista bidimensional la distribucion de cada abono
                    List<List<Prestamo_Abono>> arrPagos = movimientos.Select(mov => mov.prestamosAbonados.ToList()).ToList();
                    //Se hace la suma de todos los montos de las distribuciones hechas a intereses
                    res = arrPagos.Sum(pgs => pgs.Where(mov => mov.pagoAInteres).Sum(mov => mov.monto));
                }
            }
            return res;
        }

        /// <summary>
        /// Calcula el monto total de interes pagado por los abonos hechos dentro de la temporada indiquada en el argumento.
        /// </summary>
        /// <param name="tem">Instancia de TemporadaDeCosecha sobre la cual se quiere calcular los intereses pagados.</param>
        /// <returns></returns>
        public decimal getCapitalPagadoEnLaTemporada(TemporadaDeCosecha tem)
        {
            decimal res = 0;
            if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0)
            {
                var movimientos = this.MovimientosFinancieros
                    .Where(mov => mov.TemporadaDeCosechaID <= tem.TemporadaDeCosechaID)
                    .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS
                        && mov.isAbonoCapital).Cast<PrestamoYAbonoCapital>();
                if (movimientos.Count() > 0)
                {
                    //Se almancena dentro de una lista bidimensional la distribucion de cada abono
                    List<List<Prestamo_Abono>> arrPagos = movimientos.Select(mov => mov.prestamosAbonados.ToList()).ToList();
                    //Se hace la suma de todos los montos de las distribuciones hechas a intereses
                    res = arrPagos.Sum(pgs => pgs.Where(mov => !mov.pagoAInteres).Sum(mov => mov.monto));
                }
            }
            return res;
        }

        /// <summary>
        /// Arroja el balance correspondiente a la fecha de consulta sobre el balance de anticipos y ventas de material.
        /// </summary>
        /// <param name="fecha">Fecha de consulta sobre al cual se determinara el balance actual</param>
        /// <returns></returns>
        public decimal balanceDeAnticiposEnFecha(DateTime? dt)
        {
            decimal res = 0;
            if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0)
            {
                IEnumerable<MovimientoFinanciero> movimientos;
                if (!dt.HasValue)
                    movimientos = this.MovimientosFinancieros;
                else {
                    DateTime fecha = dt.Value.AddDays(1).AddMilliseconds(-1); //Dentro de toda la totalidad del dia
                    movimientos = this.MovimientosFinancieros.Where(mov => mov.fechaMovimiento <= fecha);
                }

                if (movimientos.Count() > 0)
                {
                    movimientos = movimientos.Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS).ToList();
                    var lastMov = movimientos.OrderByDescending(mov => mov.fechaMovimiento).FirstOrDefault();
                    res = lastMov == null ? 0 : lastMov.balance;
                }
            }
            return res;
        }

        /// <summary>
        /// Arroja el balance correspondiente a la fecha de consulta sobre el balance de anticipos y ventas de material.
        /// </summary>
        /// <param name="fecha">Fecha de consulta sobre al cual se determinara el balance actual</param>
        /// <returns></returns>
        public decimal balanceDeAnticiposEnTemporada(int temporadaID)
        {
            decimal res = 0;
            if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0)
            {
                var movimientos = this.MovimientosFinancieros.Where(mov => mov.TemporadaDeCosechaID == temporadaID);
                if (movimientos.Count() > 0)
                {
                    movimientos = movimientos.Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS).ToList();
                    var lastMov = movimientos.OrderByDescending(mov => mov.fechaMovimiento).FirstOrDefault();
                    res = lastMov == null ? 0 : lastMov.balance;
                }
            }
            return res;
        }

        [DisplayName("Balance Actual por Árboles (USD)")]
        public decimal balanceActualArboles
        {
            get
            {
                if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0)
                {
                    decimal balance = 0;
                    var movimientos = this.MovimientosFinancieros
                        .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO).ToList();
                    if (movimientos.Count() > 0) {
                        var lastMov = movimientos
                            .OrderByDescending(mov => mov.fechaMovimiento).FirstOrDefault();
                        balance = lastMov == null?0: lastMov.balance;
                    }
                    return balance;
                }
                else return 0;
            }
        }

        public decimal balanceArbolesEnFecha(DateTime? fecha)
        {
            decimal res = 0;
            if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count() > 0)
            {
                IEnumerable<MovimientoFinanciero> movimientos;
                if (!fecha.HasValue)
                    movimientos = this.MovimientosFinancieros;
                else
                    movimientos = this.MovimientosFinancieros.Where(mov => mov.fechaMovimiento <= fecha.Value);
                
                if (movimientos.Count() > 0)
                {
                    movimientos = movimientos
                        .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO).ToList();
                    var lastMov = movimientos
                        .OrderByDescending(mov => mov.fechaMovimiento).FirstOrDefault();
                    res = lastMov==null?0 : lastMov.balance;
                }
            }
            return res;
        }

        /// <summary>
        /// Se ajustan todos los balances de los movimientos del productor
        /// desde la fecha inicial indicada.
        /// </summary>
        /// <param name="fechaInicial">Fecha desde la cual se comienzan a corregir los balances.</param>
        internal int ajustarBalances(MovimientoFinanciero ultimoMovimiento, ApplicationDbContext db,
            MovimientoFinanciero.TipoDeBalance tipoBalance)
        {
            /*Primeor se filtran los movimientos dentro de un tipo de balance determinado*/
            var movimientos = this.MovimientosFinancieros
                .Where(mov => mov.tipoDeBalance == tipoBalance);

            /*Tomando como referencia el ultimo movimiento anterior al recien modificado, se toman
            todos los registros posteriores a este, en caso de que el recien modificado sea el 1ro,
            se toman por defecto todos los registros existentes*/
            movimientos = ultimoMovimiento != null ? movimientos
                    .Where(mov => mov.fechaMovimiento > ultimoMovimiento.fechaMovimiento)
                    : movimientos;

            //Se crea una lista encadenada ordenada cronologicamente hacia el pasado
            movimientos = movimientos.OrderByDescending(mov => mov.fechaMovimiento).ToList();
            LinkedList<MovimientoFinanciero> movimientosOrdenados = new LinkedList<MovimientoFinanciero>(movimientos);
            
            //Para el registro recien modificado, se recalcula su balance
            if (movimientosOrdenados.Count() > 0) {
                decimal monto = movimientosOrdenados.Last.Value.montoMovimiento;
                if (movimientosOrdenados.Last.Value.tipoDeBalance == TipoDeBalance.CAPITAL_VENTAS 
                    && movimientosOrdenados.Last.Value.isAbonoCapital)
                    monto = ((PrestamoYAbonoCapital)movimientosOrdenados.Last.Value).capitalAbonado;

                movimientosOrdenados.Last.Value.balance = monto
                    + (ultimoMovimiento == null ? 0 : ultimoMovimiento.balance);

            }

            /*Recorriendo la lista encadenada desde el registro recien modificado hasta el ultimo
            se van corrigiendo los balances, uno tras otro, esto solo cuando es mas de 1 movimiento registrado*/
            int numreg = 0;
            if (movimientos.Count() > 1)
            {
                //apuntador actual
                var nodePointer = movimientosOrdenados.Last;
                while (nodePointer.Previous != null)
                {
                    //nodo anterior
                    var nodo = nodePointer.Previous;

                    //nodo.Value.balance = nodePointer.Value.balance + nodo.Value.montoMovimiento;
                    //Calculo de interes del movimiento para ser incluido en el balance
                    decimal monto = nodo.Value.montoMovimiento;
                    if (nodo.Value.tipoDeBalance == TipoDeBalance.CAPITAL_VENTAS && nodo.Value.isAbonoCapital) { 
                        monto = ((PrestamoYAbonoCapital)nodo.Value).capitalAbonado;
                        //Si aun esta activo el el abono, se agrega al balance el monto disponible
                        if (!((PrestamoYAbonoCapital)nodo.Value).agotado)
                            monto += ((PrestamoYAbonoCapital)nodo.Value).montoActivo;
                    }

                    //Se calcula nuevo balance
                    nodo.Value.balance = nodePointer.Value.balance + monto;

                    //Se recorre apuntador
                    nodePointer = nodo;
                }
            }

            //Se notifica la edicion de los registros modificados
            movimientosOrdenados.ToList()
                .ForEach(mov => db.Entry(mov).State = System.Data.Entity.EntityState.Modified);

            //Se guardan cambios
            numreg = db.SaveChanges();

            return numreg;
        }

        /// <summary>
        /// Limpia los registros que asocian prestamo_pagos  que se ven afectados por la introduccion
        /// de un nuevo movimiento.
        /// </summary>
        /// <param name="db">Contexto de la base de datos.</param>
        /// <param name="nuevoMovimiento">Movimiento que se encuentra siendo registrado</param>
        /// <param name="editMode">Introducir TRUE si se quiere indicar que un movimiento editado 
        ///     afectara la distribucion.</param>
        /// <returns></returns>
        private int limpiarDistribuiciones(ApplicationDbContext db, MovimientoFinanciero nuevoMovimiento, bool editMode)
        {
            int numRegs = 0;
            //Si es un abono introducido antes de abonos agotados
            if(nuevoMovimiento.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS)
            {
                //Se eliminan los registros de distribucion de los abonos agotados
                //var movs = this.MovimientosFinancieros.Where(mov => mov.isAbonoCapital)
                var movs = this.MovimientosFinancieros
                    .Where(mov => mov.isAbonoCapital)
                    .Where(mov => mov.fechaMovimiento >= nuevoMovimiento.fechaMovimiento)
                    .Where(mov => mov.agotado).ToList();

                if (editMode) { 
                    //Incluir el movimiento que esta siendo editado dentro de la limpieza de distribuciones
                    //en caso de que el registro que esta siendo editado sea un abono de capital
                    if (nuevoMovimiento.isAbonoCapital)
                    {//Se agrega el movimiento que se esta editando a la lista para ser limpiado
                        var movTemp = (PrestamoYAbonoCapital)nuevoMovimiento; //Es abono, por lo que se hace casting
                        db.Entry(movTemp).Collection(mov => mov.prestamosAbonados).Load(); //Si es abono, se comprueba si ha pagado prestamos
                        if (movTemp.prestamosAbonados != null && movTemp.prestamosAbonados.Count() > 0)
                                movs.Add(movTemp);//Si, se agrega.
                    }
                    else //Se agrega el movimiento que se esta editando a la lista para ser limpiado
                    {
                        var movTemp = nuevoMovimiento;
                        db.Entry(movTemp).Collection(mov => mov.abonosRecibidos).Load();//Si es prestamo, se comprueba si ha recibido abonos
                        if (movTemp.abonosRecibidos != null && movTemp.abonosRecibidos.Count() > 0)
                            movs.Add(movTemp);//Si, se agrega
                    }
                }

                //Si existen relaciones pretamo_abono que limpiar
                if (movs.Count()>0)
                    //Se elimina cada conjunto de relaciones par que tienen entre prestamos y pagos
                    //a los movimientos filtrados
                    movs.ForEach(mov =>
                    {
                        if (mov.isAbonoCapital) { db.Prestamo_Abono.RemoveRange(((PrestamoYAbonoCapital)mov).prestamosAbonados); }
                        else { db.Prestamo_Abono.RemoveRange(mov.abonosRecibidos); }
                    });

                numRegs = db.SaveChanges();
            }

            return numRegs;
        }

        public int restaurarDistribuciones(ApplicationDbContext db)
        {
            //Se eliminan los registros de distribucion de los abonos agotados
            var movs = this.MovimientosFinancieros.Where(mov => mov.isAbonoCapital)
                .Cast<PrestamoYAbonoCapital>().ToList();

            //Si existen relaciones pretamo_abono que limpiar
            if (movs.Count() > 0)
                movs.ForEach(mov => db.Prestamo_Abono.RemoveRange(mov.prestamosAbonados));

            var nuevasAsoc = asociarAbonosConPrestamos(db);

            return nuevasAsoc.Count();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db">Contexto de la base de datos.</param>
        /// <param name="nuevoMovimiento">Movimiento que se encuentra siendo registrado</param>
        /// <param name="editMode">Introducir TRUE si se quiere indicar que un movimiento editado 
        ///     afectara la distribucion.</param>
        /// <returns></returns>
        public List<Prestamo_Abono> asociarAbonosConPrestamos(ApplicationDbContext db,
            MovimientoFinanciero nuevoMovimiento=null, bool editMode = false)
        {
            int numRegs = 0;

            //Si el movimiento que esta siendo editado es un abono, es necesario eliminar tambien
            //sus distribuciones para redistribuir.
            if(nuevoMovimiento!=null)
                numRegs = limpiarDistribuiciones(db, nuevoMovimiento, editMode);

            //TODO: Filtrar para movimientos aun no agotados (suma de abonos o pagos igual al monto del movimiento)
            var movimientosCapital = this.MovimientosFinancieros.ToList()
                .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS)
                .Where(mov => !mov.agotado).ToList();

            //Se toman todos los prestamos activos (en ventas y anticipos)
            var prestamos = new LinkedList<MovimientoFinanciero>(movimientosCapital.Where(mov => !mov.isAbonoCapital)
                .OrderBy(mov => mov.fechaMovimiento));

            //Se toman todos los abonos activos
            var abonos = new LinkedList<PrestamoYAbonoCapital>(movimientosCapital.Where(mov => mov.isAbonoCapital)
                .Cast<PrestamoYAbonoCapital>().OrderBy(mov => mov.fechaMovimiento));

            List<Prestamo_Abono> nuevasAsociaciones = new List<Prestamo_Abono>();

            //Variables iniciales para distribucion de abonos
            Boolean? pagarInteres = true;
            do //Se realiza ciclo paga barrer con los prestamos 2 veces, la 1ra para abonar intereses y la 2da para abonar capital
            {
                var prestamoNodo = prestamos.First;
                var abonoNodo = abonos.First;

                //Variables para calcular intereses por prestamo a la fecha del abono
                VMInteres interesReg = new VMInteres();
                decimal interesAlAbonar = 0, interesTotalGenerado = 0 ;bool hayInteres;

                //Si existen prestamos y abonos activos con monto activo disponible, se procede a asociarlos
                while (prestamoNodo != null && abonoNodo != null)
                {
                    MovimientoFinanciero prestamo = prestamoNodo.Value;
                    PrestamoYAbonoCapital abono = abonoNodo.Value;

                    interesAlAbonar = 0;
                    //Se determina el interes a la fecha en la que se hizo el abono
                    //No pagan interes las ventas de material ni las deudas iniciales de venta de material
                    if (pagarInteres.Value && (prestamo.getTypeOfMovement() != TypeOfMovements.VENTA_A_CREDITO 
                            && !prestamo.isAdeudoInicialMaterial))
                    { //Ciclo de pago de interes
                        interesReg = prestamo.getInteresReg(abono.fechaMovimiento, out interesTotalGenerado);
                        interesAlAbonar = interesReg.interesRestante;
                    }
                    hayInteres = Math.Round(interesAlAbonar,2) > 0;

                    //Se crea un nuevo par de asociacion
                    Prestamo_Abono pa = new Prestamo_Abono(prestamo, abono);
                    pa.pagoAInteres = hayInteres;

                    //Si es pago a intereses, se crea un registro de pago a interes
                    if (pagarInteres.Value)//Ciclo de pago de interes
                        if (hayInteres) { 
                            //Si el abono se agota al pagar el prestamo
                            if (interesAlAbonar >= abono.montoActivo)
                            {
                                pa.monto = abono.montoActivo; //El monto del par es el resto del abono
                                abonoNodo = abonoNodo.Next; //Se pasa a un siguiente abono
                            }
                            else //Si el prestamo se agota al recibir el abono
                            {
                                pa.monto = interesAlAbonar; // El monto del par es el resto del prestamo
                                prestamoNodo = prestamoNodo.Next; // Se pasa a un siguiente prestamo
                            }
                            db.Prestamo_Abono.Add(pa); //Se guarda el par asociativo de prestamo y abono
                            nuevasAsociaciones.Add(pa);
                        }
                        else //El interes del prestamo esta pagado, se pasa al siguiente prestamo
                            prestamoNodo = prestamoNodo.Next;

                    else //Si no es pago a interes, se paga capital
                    { 
                        if (prestamo.montoActivo >= abono.montoActivo)//Si el abono se agota al pagar el prestamo
                        {
                            pa.monto = abono.montoActivo; //El monto del par es el resto del abono
                            abonoNodo = abonoNodo.Next; //Se pasa a un siguiente abono
                            //Si al proceder al siguiente abono, este es de otro mes, se deben considerar
                            //los intereses generados por los prestamos en este mes, por lo que se 
                            //levanta la vandera de pagarInteres otra vez.
                            if (abonoNodo!=null && abonoNodo.Value.fechaMovimiento.Month
                                != abonoNodo.Previous.Value.fechaMovimiento.Month)
                                pagarInteres = true;
                        }
                        else //Si el prestamo se agota al recibir el abono
                        {
                            pa.monto = prestamo.montoActivo; // El monto del par es el resto del prestamo
                            prestamoNodo = prestamoNodo.Next; // Se pasa a un siguiente prestamo
                        }
                        db.Prestamo_Abono.Add(pa); //Se guarda el par asociativo de prestamo y abono
                        nuevasAsociaciones.Add(pa);
                    }

                }//Fin de ciclo mientras prestamo y abono no sean nulos
                
                if (pagarInteres.Value) { 
                    pagarInteres = false;//Transicion de pago de interes a pago de capital
                    prestamos = new LinkedList<MovimientoFinanciero>(prestamos.Where(mov=>!mov.agotado).OrderBy(mov=>mov.fechaMovimiento));
                    abonos = new LinkedList<PrestamoYAbonoCapital>(abonos.Where(mov => !mov.agotado).OrderBy(mov => mov.fechaMovimiento));
                }
                else
                    pagarInteres = null;//Transicion de pago de capital terminar ciclo

            } while (pagarInteres != null);
            
            numRegs = db.SaveChanges(); //Se guardan todos los pares marcados
            return nuevasAsociaciones;
        }

        /// <summary>
        /// Calcula el interes total que debe un productor a la fecha consultada
        /// </summary>
        /// <param name="fecha">Desde la fecha indicada hacia atras, se suman los intereses pendientes de ser liquidados.</param>
        /// <returns></returns>
        public decimal interesTotal(int tempID, DateTime? fecha, out decimal interesTotalGenerado)
        {
            decimal interesTotalQueSeDebe = 0;
            interesTotalGenerado = 0;
            DateTime dt = fecha.HasValue ? fecha.Value : DateTime.Today;
            dt = dt.AddDays(1).AddMilliseconds(-1); //Hasta el final del dia
            var movs = this.MovimientosFinancieros.Where(mov => mov.TemporadaDeCosechaID == tempID);
            if (movs != null && movs.Count() > 0)
            {
                movs = movs.Where(mov => mov.isAnticipoDeCapital || mov.isAdeudoInicialAnticiposCapital);
                foreach (var mov in movs)
                {
                    decimal interesGenerado = 0;
                    decimal interesDebido = mov.getInteresRestante(dt, out interesGenerado);
                    interesTotalQueSeDebe += interesDebido;
                    interesTotalGenerado += interesGenerado;
                }
            }

            return interesTotalQueSeDebe;
        }

        /// <summary>
        /// Calcula el interes total que debe un productor a la fecha consultada dentro de una temporada
        /// </summary>
        /// <param name="tempID">Se especifica el ID de la temporada dentro de la cual se calculara el total</param>
        /// <param name="fecha">Desde la fecha indicada hacia atras, se suman los intereses pendientes de ser liquidados</param>
        /// <returns></returns>
        public decimal interesTotalPorPagarALaFecha(int tempID, DateTime? fecha)
        {
            decimal interesTotalQueSeDebe = 0;
            DateTime dt = fecha.HasValue ? fecha.Value : DateTime.Today;
            dt = dt.AddDays(1).AddMilliseconds(-1); //Hasta el final del dia
            var movs = this.MovimientosFinancieros.Where(mov => mov.TemporadaDeCosechaID == tempID);
            if (movs != null && movs.Count() > 0)
            {
                movs = movs.Where(mov => mov.isAnticipoDeCapital || mov.isAdeudoInicialAnticiposCapital);
                foreach (var mov in movs)
                {
                    decimal interesGenerado = 0;
                    decimal interesDebido = mov.getInteresRestante(dt, out interesGenerado);
                    interesTotalQueSeDebe += interesDebido;
                }
            }

            return interesTotalQueSeDebe;
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}. Zona: {2}", this.numProductor, this.nombreProductor, this.zona);
        }

        public static class Zonas
        {
            public const string ZONA1 = "Caborca";
            public const string ZONA2 = "Baja";
        }

        /// <summary>
        /// Enumera las columnas del excel de productores.
        /// </summary>
        public enum ExcelColumns
        {
            NUM = 0, NOMBRE=1,
            DIRECCION =2, RFC=3,
            ZONA = 4, NOMBRE_DEL_CHEQUE=5,
            REPRESENTANTE_LEGAL=6
        }

        public static int importarProductores(HttpPostedFileBase xlsFile, ApplicationDbContext db,
            out List<ExcelTools.ExcelParseError> errores, out ExcelTools.ExcelParseError errorGeneral)
        {
            int regsSaved = 0;
            //Lista para recoleccion de errores
            errores = new List<ExcelTools.ExcelParseError>();
            errorGeneral = new ExcelTools.ExcelParseError();
            //Se verifica la validez del archivo recibido
            if ((xlsFile != null) && (xlsFile.ContentLength > 0) && !string.IsNullOrEmpty(xlsFile.FileName))
            {
                //Se toman los datos del archivo
                string fileName = xlsFile.FileName;//nombre
                string fileContentType = xlsFile.ContentType;//tipo
                byte[] fileBytes = new byte[xlsFile.ContentLength];//composicion en bytes
                var data = xlsFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(xlsFile.ContentLength)); //datos a leer

                //Se crea el archivo Excel procesable
                var package = new ExcelPackage(xlsFile.InputStream);
                //var workSheet = currentSheet.First();//Se toma la 1ra hoja de excel
                var workSheet = package.Workbook.Worksheets["Productores"];//Se toma la 1ra hoja de excel
                var noOfCol = workSheet.Dimension.End.Column;//Se determina el ancho de la tabla en no. columnas
                var noOfRow = workSheet.Dimension.End.Row;//El alto de la tabla en numero de renglores

                ExcelTools.ExcelParseError error = new ExcelTools.ExcelParseError();
                //Se recorre cada renglon de la hoja
                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    //Se toma renglon
                    var rowProductor = workSheet.Cells[rowIterator, 1, rowIterator, noOfCol];
                    //Se convierte renglon en registro para DB con registro de errores
                    var productorReg = new Productor(rowProductor, ref error);

                    if (!error.isError) //Si la instancia se creo satisfactoriamente basada de un renglon de excel
                    {
                        //Se busca si ya existe el productor con el mismo numero en la base de datos
                        var productorDB = db.Productores.ToList()
                            .FirstOrDefault(rp => rp.numProductor == productorReg.numProductor);
                        //Si el registro no existe, se agrega
                        if (productorDB == null)
                            db.Productores.Add(productorReg);
                        else
                        {
                            //Si ya existe, se identifica con el mismo ID y se marca como modificado
                            productorReg.idProductor = productorDB.idProductor;
                            db.Entry(productorDB).State = System.Data.Entity.EntityState.Detached;
                            db.Entry(productorReg).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    else
                        errores.Add(error);
                }
                
                regsSaved = db.SaveChanges();
            }
            return regsSaved;
        }

        /// <summary>
        /// Arroja aregglo de zones enumeradas en la clase estatica Zonas.
        /// </summary>
        /// <returns>Arreglo de objectos dinamicos con Text y Value como atributos,
        /// ambos con el nombre de la zona.</returns>
        public static object[] getZonasItemArray()
        {
            object[] array = {
                new { Text=Zonas.ZONA1, Value=Zonas.ZONA1},
                new { Text=Zonas.ZONA2, Value=Zonas.ZONA2}
            };
            return array;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selVal">Optional, valor seleccionado por defecto.</param>
        /// <returns>Select list listo para ser usado en la vista rellenado con las zonas</returns>
        public static SelectList getZonasSelectList(object selVal = null)
        {
            return new SelectList(Productor.getZonasItemArray(), "Value", "Text", selVal);
        }

        public MovimientoFinanciero getUltimoMovimiento()
        {
            MovimientoFinanciero m = this.MovimientosFinancieros
                .Where(mov => mov.isAbonoOPrestamo())
                .OrderByDescending(mov => mov.fechaMovimiento).FirstOrDefault();

            return m;
        }

        /// <summary>
        /// Busca el ultimo movimiento hecho en la fecha indicada por el argumento.
        /// 
        /// </summary>
        /// <param name="fechaMovimiento"></param>
        /// <returns></returns>
        internal MovimientoFinanciero getUltimoMovimiento(DateTime fechaMovimiento, MovimientoFinanciero.TipoDeBalance tipoBalance)
        {
            //Se busca el ultimo movimiento anterior a la fecha de referencia dentro del mismo tipo de balance
            var movs = this.MovimientosFinancieros
                .Where(mov => tipoBalance == MovimientoFinanciero.TipoDeBalance.NONE?
                    true : mov.tipoDeBalance == tipoBalance)
                .Where(mov => mov.fechaMovimiento < fechaMovimiento)
                .OrderByDescending(mov => mov.fechaMovimiento)
                .Take(2).ToList();

            MovimientoFinanciero m;
            //Si hay resultados, se toma el resultado
            if (movs.Count() > 0) {
                m = movs.FirstOrDefault();
                //m = movs.ElementAt(movs.Count() - 1);
            }
            else // Si no, se establece como nulo
                m = null;

            return m;
        }

        /// <summary>
        /// Genera una lista de movimientos financieros correspondientes al balance de compras de arboles de olivo.
        /// </summary>
        /// <param name="tem">Temporada actual</param>
        /// <param name="db">Contexto de la base de datos</param>
        /// <param name="totales">Instancia de VMTotalesSimple donde se guarda un reporte de los totales del reporte</param>
        /// <returns></returns>
        public List<MovimientoFinanciero> generarReporteVentasArboles(TemporadaDeCosecha tem, ApplicationDbContext db, 
            ref VMTotalesSimple totales)
        {
            TemporadaDeCosecha temporadaAnterior = tem.getTemporadaAnterior(db);
            return generarReporteVentasArboles(tem,temporadaAnterior, ref totales);
        }

        /// <summary>
        //Genera una lista en orden cronoligo de todos los movimientos de compras y abonos a arboles de olivo
        //incluyendo tambien el global de temporadas anteriores
        /// </summary>
        /// <param name="tem">Referencia a la instancia del año de cosecha actual</param>
        /// <param name="temporadaAnterior">Referencia a la instancia del año de cosecha anterior</param>
        /// <param name="totales">Argumento por referencia donde se almacenaran la suma de los campos principales del reporte.</param>
        /// <returns></returns>
        public List<MovimientoFinanciero> generarReporteVentasArboles(TemporadaDeCosecha tem, TemporadaDeCosecha temporadaAnterior,
            ref VMTotalesSimple totales)
        {
            var res = this.MovimientosFinancieros;
            //Genera una lista de todos los movimientos de compras de arboles en el presente año
            if (res != null && res.Count() > 0)
            {
                res = res.Where(mov => mov.TemporadaDeCosechaID == tem.TemporadaDeCosechaID)
                .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO)
                .OrderBy(mov => mov.fechaMovimiento).ToList();
            }
            //Si existe un año anterior, agrega un movimiento inicial nuevo englobando el balance resultante de ese año
            //para el concepto de compras de arboles y lo agrega a la lista
            if (temporadaAnterior != null)
            {
                decimal balanceAnteriorArbolesOlivo = this.getBalanceArbolesOlivo(temporadaAnterior.TemporadaDeCosechaID);
                if (Math.Abs(balanceAnteriorArbolesOlivo) > 0)
                {
                    AdeudoInicial adeudoInicialArbolesOlivoDesdeTemporadaAnterior = new AdeudoInicial(balanceAnteriorArbolesOlivo,
                        tem, this, true, TipoDeBalance.VENTA_OLIVO);
                    //Se le asigna al movimiento de deuda inicial la fecha final de la temporada anterior
                    adeudoInicialArbolesOlivoDesdeTemporadaAnterior.fechaMovimiento = tem.fechaInicio;
                    LinkedList<MovimientoFinanciero> tempRes = new LinkedList<MovimientoFinanciero>(res);
                    tempRes.AddFirst(adeudoInicialArbolesOlivoDesdeTemporadaAnterior);
                    res = tempRes.ToList();
                }
            }
            //De la lista generada, se acumulan los totales en uns instancia VMTotalesSimple
            totales = new VMTotalesSimple(res.ToList());
            return res.ToList();
        }

        /// <summary>
        /// Determina el balance resultante dentro de un año de cosecha correspondiente a el estado de cuenta de compra de arboles
        /// para la instancia del productor.
        /// </summary>
        /// <param name="temporadaDeCosechaID"></param>
        /// <returns></returns>
        public decimal getBalanceArbolesOlivo(int temporadaDeCosechaID)
        {
            decimal res = 0;
            var movs = this.MovimientosFinancieros.Where(mov => mov.TemporadaDeCosechaID == temporadaDeCosechaID);
            //Se toman solamente los movimientos correpondientes a compras de arboles dentro de un año de cosecha especifico
            if (movs != null && movs.Count() > 0)
            {
                movs = movs.Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO)
                .OrderBy(mov => mov.fechaMovimiento).ToList();
            }
            //Se determina el balance final resultante del ultimo movimiento dentro del año
            if (movs.Count() > 0)
                res = movs.Last().balance;

            return res;
        }

        /// <summary>
        /// Metodo recursivo que genera un reporte de anticipos e intereses dados dentro de una temporada. Incluye adeudo de temporada anterior, lo cual
        /// se determina al vuelo tomando cada una de las temporadas anteriores a la actual de forma recursiva.
        /// </summary>
        /// <param name="fechaActual">Fecha a la cual se calculan los balances para determinar intereses.</param>
        /// <param name="temporadaSeleccionada">Temporada reportada</param>
        /// <param name="db">Contexto de la base de datos</param>
        /// <returns></returns>
        public IEnumerable<MovimientoFinanciero.VMMovimientoBalanceAnticipos> generarReporteAnticiposConIntereses(DateTime fechaActual,
            TemporadaDeCosecha temporadaSeleccionada, ApplicationDbContext db)
        {
            TemporadaDeCosecha temporadaAnterior = temporadaSeleccionada == null ? null : temporadaSeleccionada.getTemporadaAnterior(db);
            if (temporadaAnterior == null && temporadaSeleccionada == null)
            {
                return new List<VMMovimientoBalanceAnticipos>();
            }
            else
                return generarReporteAnticiposConIntereses(fechaActual, temporadaSeleccionada, temporadaAnterior);
        }

        /// <summary>
        /// Genera un reporte de anticipos e intereses dados dentro de una temporada
        /// </summary>
        /// <param name="fechaActual">Fecha a la cual se calculan los balances para determinar intereses.</param>
        /// <param name="temporadaSeleccionada">Temporada reportada</param>
        /// <param name="db">Contexto de la base de datos</param>
        /// <returns></returns>
        public IEnumerable<MovimientoFinanciero.VMMovimientoBalanceAnticipos> generarReporteAnticiposConIntereses(DateTime fechaActual,
            TemporadaDeCosecha temporadaSeleccionada, TemporadaDeCosecha temporadaAnterior)
        {

            //Solo prestamos y abonos
            var movimientosFiltrados = this.MovimientosFinancieros
            .Where(mov => mov.TemporadaDeCosechaID == temporadaSeleccionada.TemporadaDeCosechaID)
            .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS)
            .OrderBy(mov => mov.fechaMovimiento).ToList();

            //Se prepara un reporte de movimientos de anticipos con los movimientos filtrados
            var movs = from mov in movimientosFiltrados
                       select new MovimientoFinanciero.VMMovimientoBalanceAnticipos(mov, fechaActual);

            //Se genera una lista encandenada de los movimientos filtrados
            LinkedList<MovimientoFinanciero.VMMovimientoBalanceAnticipos> movimientos =
                        new LinkedList<MovimientoFinanciero.VMMovimientoBalanceAnticipos>(movs);


            //TODO: Deteminar el adeudo de la temporada anterior o los movimientos de ese tipo
            List<AdeudoInicial> adeudos = this.calcularAdeudoFinal(temporadaAnterior, fechaActual);
            AdeudoInicial adeudoAnticipos = adeudos.Count() > 0 ? adeudos.ElementAt(0) : null;
            AdeudoInicial adeudoPorVentas = adeudos.Count() > 1 ? adeudos.ElementAt(1) : null;

            agregarALaVistaDeBalances(adeudoPorVentas, fechaActual, ref movimientos);
            agregarALaVistaDeBalances(adeudoAnticipos, fechaActual, ref movimientos);

            //Se calcula el balance de deuda
            MovimientoFinanciero.VMMovimientoBalanceAnticipos.balancear(ref movimientos, 0);

            return movimientos;
        }

        private void agregarALaVistaDeBalances(AdeudoInicial adeudo, DateTime fechaActual,
            ref LinkedList<VMMovimientoBalanceAnticipos> movimientos)
        {
            //Si el calculo de adeudo no es nulo, se construye una vista del registro para ser agregada a la lista de balances
            VMMovimientoBalanceAnticipos vmAdeudoAnteriorAnticipos = adeudo == null ? null :
                vmAdeudoAnteriorAnticipos = new VMMovimientoBalanceAnticipos(adeudo, fechaActual);

            //El registro solamente se agrega a la vita si no es nulo y si alguno de sus campos de moneda no es cero
            if (vmAdeudoAnteriorAnticipos != null && (Math.Abs(Math.Round(vmAdeudoAnteriorAnticipos.anticipo, 2)) > 0
                || Math.Abs(Math.Round(vmAdeudoAnteriorAnticipos.interes, 2)) > 0))
            {
                //Se agrega el movimiento de adeudo anterior al inicio de la lista
                movimientos.AddFirst(vmAdeudoAnteriorAnticipos);
            }
        }

        /// <summary>
        /// Genera un reporte donde muestra el total de ingresos de cosecha tomando todos los recibos registrados
        /// dados como argumento
        /// </summary>
        /// <param name="movs">Lista de recibos a reportar.</param>
        /// <param name="productos">Lista detallada de los productos que maneja el productor</param>
        /// <param name="precioDolar">Precio del dolar que se quiere considerar en este reporte</param>
        /// <param name="temporadaID">ID de la temporada correspondiente</param>
        /// <returns></returns>
        public List<RecepcionDeProducto.VMTotalDeEntregas> generarReporteSemanalIngresosCosecha(List<PagoPorProducto> movs,
            List<VMTipoProducto> productos, decimal precioDolar, int temporadaID = 0)
        {
            if (movs == null && temporadaID>0)
            {
                movs = this.MovimientosFinancieros.Where(mov => mov.getTypeOfMovement() == TypeOfMovements.PAGO_POR_PRODUCTO)
                    .Where(mov => mov.TemporadaDeCosechaID == temporadaID).Cast<PagoPorProducto>().ToList();
            }
            var totales = this.getTotalEntregas(movs);
            var report = new List<RecepcionDeProducto.VMTotalDeEntregas>();
            /*Por cada producto calculado sus totales de entrega y ganancias, se genera una tabla acorde al reporte semanal de liquidacion
            mensual marcado por el VMTotalDeEntregas con las columnas "Variedad", Precio Por tonelada, Toneladas Entregadas, Valor en USD,
            valor en pesos, valor total de cosecha en USD*/
            foreach(var producto in productos)
            {
                var total = totales.FirstOrDefault(tot => tot.producto == producto.producto);
                double cantidadTotal = total==null?0:total.cantidad;
                RecepcionDeProducto.VMTotalDeEntregas totalReg = new RecepcionDeProducto.VMTotalDeEntregas() { producto = producto.producto,
                precio = producto.precio, monto = producto.precio*(decimal)cantidadTotal,
                    montoMXN = producto.precio * (decimal)cantidadTotal * precioDolar, toneladasRecibidas = cantidadTotal
                };

                report.Add(totalReg);
            }

            return report;
        }

        /// <summary>
        /// Determina el adeudo final resultante de una temporada dada para este productor, calculando los intereses a la fecha dada.
        /// </summary>
        /// <param name="temporadaActual">Temporada sobre la cual se calculará la deuda final.</param>
        /// <param name="fechaActual">Fecha que se toma como referencia para el cálculo de los intereses de los anticipos.</param>
        /// <returns>Regresa un arreglo de 2 elementos, el 1ro representa el adeudo final correspondiente a los anticipos de capital, el 2do elemento
        /// corresponde a el adeudo por ventas de material.</returns>
        private List<AdeudoInicial> calcularAdeudoFinal(TemporadaDeCosecha temporadaActual, DateTime? fechaActual)
        {
            DateTime fecha = fechaActual.HasValue ? fechaActual.Value : DateTime.Today;
            ApplicationDbContext db = new ApplicationDbContext();
            TemporadaDeCosecha temporadaAnterior = temporadaActual==null?null:temporadaActual.getTemporadaAnterior(db);
            AdeudoInicial adeudoAnticipo = null, adeudoPorVentas = null; //Por defecto, el adeudo inciial a regresar será 0
            List<AdeudoInicial> adeudos = new List<AdeudoInicial>();
            if (temporadaActual != null)//Si la temporada actual no es nula, se genera su reporte de balances de anticipos 
            {
                var reporte = this.generarReporteAnticiposConIntereses(fecha, temporadaActual, temporadaAnterior);
                VMMovimientoBalanceAnticipos.VMBalanceAnticiposTotales totales = new VMMovimientoBalanceAnticipos.VMBalanceAnticiposTotales(reporte);
                //En base a los totales del reporte, se genera un movimiento de adeudo inicial
                //TODO: Ajustar el contructor para solo ventas o solo anticipos
                adeudoAnticipo = new AdeudoInicial(totales, temporadaActual, this);
                adeudoPorVentas = new AdeudoInicial(totales, temporadaActual, this,true);
                adeudos.Add(adeudoAnticipo);
                adeudos.Add(adeudoPorVentas);
            }

            return adeudos;
        }

        public List<PagoPorProducto> filtrarPagosPorProducto(TemporadaDeCosecha tem, TimePeriod tp, int noSemana)
        {
            //Se filtran los movimientos dentro del periodo de cosecha, que sean pagos por producto dentro
            //del periodo de tiempo consultado
            List<PagoPorProducto> movs = this.MovimientosFinancieros
                .Where(mov => mov.TemporadaDeCosechaID == tem.TemporadaDeCosechaID).ToList() //De la temporada consultada
                .Where(mov => mov.getTypeOfMovement() == MovimientoFinanciero.TypeOfMovements.PAGO_POR_PRODUCTO) //Pagos por producto
                .Where(mov => ((PagoPorProducto)mov).semana == noSemana) //De cierta semana
                .Where(mov => tp.hasInside(mov.fechaMovimiento)).Cast<PagoPorProducto>().ToList(); //Dentro de cierto rango de tiempo

            return movs;
        }

        private List<VMTipoProducto> getTotalEntregas(List<PagoPorProducto> movs)
        {

            PagoPorProducto total = new PagoPorProducto();
            //Se reporta dentro de un registro de PagoPorProducto la suma de todas las cantidades y precios
            //filtrados
            if (movs != null) { 
                total.pagoProducto1 = movs.Sum(mov => ((PagoPorProducto)mov).pagoProducto1);
                total.pagoProducto2 = movs.Sum(mov => ((PagoPorProducto)mov).pagoProducto2);
                total.pagoProducto3 = movs.Sum(mov => ((PagoPorProducto)mov).pagoProducto3);
                total.pagoProducto4 = movs.Sum(mov => ((PagoPorProducto)mov).pagoProducto4);
                total.pagoProducto5 = movs.Sum(mov => ((PagoPorProducto)mov).pagoProducto5);
                total.pagoProducto6 = movs.Sum(mov => ((PagoPorProducto)mov).pagoProducto6);
                total.cantidadProducto1 = movs.Sum(mov => ((PagoPorProducto)mov).cantidadProducto1);
                total.cantidadProducto2 = movs.Sum(mov => ((PagoPorProducto)mov).cantidadProducto2);
                total.cantidadProducto3 = movs.Sum(mov => ((PagoPorProducto)mov).cantidadProducto3);
                total.cantidadProducto4 = movs.Sum(mov => ((PagoPorProducto)mov).cantidadProducto4);
                total.cantidadProducto5 = movs.Sum(mov => ((PagoPorProducto)mov).cantidadProducto5);
                total.cantidadProducto6 = movs.Sum(mov => ((PagoPorProducto)mov).cantidadProducto6);
            }
            //Se presenta la informacion en una lista de 3 tipos de productos con su correspondiente cantidad, precio y nombre
            List<VMTipoProducto> totalesProducto = new List<VMTipoProducto>();
            totalesProducto.Add(new VMTipoProducto
            {
                producto = TemporadaDeCosecha.TiposDeProducto.PRODUCTO1,
                precio = total.pagoProducto1,
                cantidad = total.cantidadProducto1
            });
            totalesProducto.Add(new VMTipoProducto
            {
                producto = TemporadaDeCosecha.TiposDeProducto.PRODUCTO2,
                precio = total.pagoProducto2,
                cantidad = total.cantidadProducto2
            });
            totalesProducto.Add(new VMTipoProducto
            {
                producto = TemporadaDeCosecha.TiposDeProducto.PRODUCTO3,
                precio = total.pagoProducto3,
                cantidad = total.cantidadProducto3
            });
            totalesProducto.Add(new VMTipoProducto
            {
                producto = TemporadaDeCosecha.TiposDeProducto.PRODUCTO4,
                precio = total.pagoProducto4,
                cantidad = total.cantidadProducto4
            });
            totalesProducto.Add(new VMTipoProducto
            {
                producto = TemporadaDeCosecha.TiposDeProducto.PRODUCTO5,
                precio = total.pagoProducto5,
                cantidad = total.cantidadProducto5
            });
            totalesProducto.Add(new VMTipoProducto
            {
                producto = TemporadaDeCosecha.TiposDeProducto.PRODUCTO6,
                precio = total.pagoProducto6,
                cantidad = total.cantidadProducto6
            });

            return totalesProducto;
        }

        public class VMLiberacionRetencionesStatus
        {
            public Productor productor { get; set; }
            public TemporadaDeCosecha temporada { get; set; }
            public Retencion.TipoRetencion tipoRetencion { get; set; }
            public List<RetencionCheque> cheques { get; set; }

            public VMLiberacionRetencionesStatus() { }
            public VMLiberacionRetencionesStatus(Productor productor, TemporadaDeCosecha temporada, 
                Retencion.TipoRetencion tipoRetencion)
            {
                this.productor = productor;
                this.temporada = temporada;
                this.tipoRetencion = tipoRetencion;
                this.cheques = this.productor.cheques.Where(ch => ch.tipoDeDeduccion == tipoRetencion).ToList();
            }

            [DisplayName("Monto Liberado")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal montoLiberado
            {
                get
                {
                    decimal res = 0;
                    if (this.cheques != null && this.cheques.Count() > 0)
                        res = this.productor.cheques.Where(ch => ch.tipoDeDeduccion == this.tipoRetencion
                        && ch.temporadaID == temporada.TemporadaDeCosechaID).Sum(mov => mov.monto);

                    return res;
                }
            }

            [DisplayName("Monto Total Retenido")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal montoTotalRetenido {
                get{
                    decimal res = 0;
                    var movs = this.productor.MovimientosFinancieros
                        .Where(mov => mov.TemporadaDeCosechaID == temporada.TemporadaDeCosechaID
                            && mov.getTypeOfMovement() == TypeOfMovements.RENTENCION
                            && ((Retencion)mov).tipoDeDeduccion == this.tipoRetencion)
                        .ToList();

                    if (movs != null && movs.Count() > 0)
                    { res = movs.Sum(mov => mov.montoMovimiento); }

                    return Math.Abs(res);
                }
            }

            [DisplayName("Monto aún retenido")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal montoAunRetenido
            {
                get { return Math.Abs(this.montoTotalRetenido) - Math.Abs(this.montoLiberado);  }
            }

            [DisplayName("Pagada")]
            public bool isRetencionesPagadas
            {
                get { return this.montoAunRetenido <= 0; }
            }

            [DisplayName("Retención")]
            public string nombreTipoRetencion { get {
                    return Retencion.getNombreTipoRetencion(this.tipoRetencion);
                } }
        }
    }
}