using System;
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

namespace CampanasDelDesierto_v1.Models
{
    public class Productor
    {
        [Key]
        public int idProductor { get; set; }

        [Display(Name = "Número de Productor")]
        public string numProductor { get; set; }

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

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Adeudo Anterior (USD)")]
        public decimal? adeudoAnterior { get; set; }

        [DisplayName("Balance Actual (USD)")]
        public decimal balanceActual
        {
            get
            {
                var movimientos = this.MovimientosFinancieros;
                if (movimientos != null && movimientos.Count() > 0)
                {
                    movimientos = movimientos.Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.CAPITAL_VENTAS).ToList();
                    decimal balance = movimientos
                        .OrderByDescending(mov => mov.fechaMovimiento).FirstOrDefault().balance;
                    return balance;
                }
                else return 0;
            }
        }

        [DisplayName("Balance Actual por Árboles (USD)")]
        public decimal balanceActualArboles
        {
            get
            {
                var movimientos = this.MovimientosFinancieros
                    .Where(mov => mov.tipoDeBalance == MovimientoFinanciero.TipoDeBalance.VENTA_OLIVO).ToList();
                if (movimientos != null && movimientos.Count() > 0)
                {
                    decimal balance = movimientos
                        .OrderByDescending(mov => mov.fechaMovimiento).FirstOrDefault().balance;
                    return balance;
                }
                else return 0;
            }
        }

        public virtual ICollection<MovimientoFinanciero> MovimientosFinancieros { get; set; }

        //Los productores tienen recepciones de producto
        public virtual ICollection<RecepcionDeProducto> recepcionesDeProducto { get; set; }

        public Productor() { }

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
        public List<Prestamo_Abono> asociarAbonosConPrestamos(ApplicationDbContext db, MovimientoFinanciero nuevoMovimiento=null, bool editMode = false)
        {
            int numRegs = 0;
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
                decimal interesAlAbonar = 0;bool hayInteres;

                //Si existen prestamos y abonos activos con monto activo disponible, se procede a asociarlos
                while (prestamoNodo != null && abonoNodo != null)
                {
                    MovimientoFinanciero prestamo = prestamoNodo.Value;
                    PrestamoYAbonoCapital abono = abonoNodo.Value;

                    //Se determina el interes a la fecha en la que se hizo el abono
                    if (pagarInteres.Value) { //Ciclo de pago de interes
                        interesReg = prestamo.getInteresReg(abono.fechaMovimiento);
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
                        { 
                            prestamoNodo = prestamoNodo.Next;
                        }

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

        public override string ToString()
        {
            return String.Format("{0}: {1}. Zona: {2}", this.numProductor, this.nombreProductor, this.zona);
        }

        public static class Zonas
        {
            public const string ZONA1 = "Caborca";
            public const string ZONA2 = "Baja";
        }

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

                    if (!error.isError)
                    {
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
            if (movs.Count() > 0)
                m = movs.ElementAt(movs.Count() - 1);
            else // Si no, se establece como nulo
                m = null;

            return m;
        }

        public List<RecepcionDeProducto.VMTotalDeEntregas> generarReporteSemanalIngresosCosecha(List<PagoPorProducto> movs,List<VMTipoProducto> productos, decimal precioDolar)
        {
            var totales = this.getTotalEntregas(movs);
            var report = new List<RecepcionDeProducto.VMTotalDeEntregas>();
            /*Por cada producto calculado sus totales de entrega y ganancias, se genera una tabla acorde al reporte semanal de liquidacion
            mensual marcado por el VMTotalDeEntregas con las columnas "Variedad", Precio Por tonelada, Toneladas Entregadas, Valor en USD,
            valor en pesos, valor total de cosecha en USD*/
            foreach(var producto in productos)
            {
                var total = totales.FirstOrDefault(tot => tot.producto == producto.producto);
                RecepcionDeProducto.VMTotalDeEntregas totalReg = new RecepcionDeProducto.VMTotalDeEntregas() { producto = producto.producto,
                precio = producto.precio, monto = producto.precio*(decimal)total.cantidad,
                    montoMXN = producto.precio * (decimal)total.cantidad*precioDolar, toneladasRecibidas = total.cantidad};
                report.Add(totalReg);
            }

            return report;
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
            total.pagoProducto1 = movs.Sum(mov => ((PagoPorProducto)mov).pagoProducto1);
            total.pagoProducto2 = movs.Sum(mov => ((PagoPorProducto)mov).pagoProducto2);
            total.pagoProducto3 = movs.Sum(mov => ((PagoPorProducto)mov).pagoProducto3);
            total.cantidadProducto1 = movs.Sum(mov => ((PagoPorProducto)mov).cantidadProducto1);
            total.cantidadProducto2 = movs.Sum(mov => ((PagoPorProducto)mov).cantidadProducto2);
            total.cantidadProducto3 = movs.Sum(mov => ((PagoPorProducto)mov).cantidadProducto3);
            //Se presenta la informacion en una lista de 3 tipos de productos con su correspondiente cantidad, precio y nombre
            List<VMTipoProducto> totalesProducto = new List<VMTipoProducto>();
            totalesProducto.Add(new VMTipoProducto
            {
                producto = TemporadaDeCosecha.TiposDeProducto.PRODUCTO1,
                precio = total.pagoProducto1, cantidad = total.cantidadProducto1
            });
            totalesProducto.Add(new VMTipoProducto
            {
                producto = TemporadaDeCosecha.TiposDeProducto.PRODUCTO2,
                precio = total.pagoProducto2, cantidad = total.cantidadProducto2
            });
            totalesProducto.Add(new VMTipoProducto
            {
                producto = TemporadaDeCosecha.TiposDeProducto.PRODUCTO3,
                precio = total.pagoProducto3, cantidad = total.cantidadProducto3
            });

            return totalesProducto;
        }
    }
}