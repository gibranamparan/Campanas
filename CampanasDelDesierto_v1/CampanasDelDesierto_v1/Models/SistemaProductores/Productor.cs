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

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name ="Adeudo Anterior (USD)")]
        public decimal? adeudoAnterior { get; set; }

        public decimal balanceActual {
            get {
                if (this.MovimientosFinancieros != null && this.MovimientosFinancieros.Count()>0) { 
                    return this.MovimientosFinancieros
                        .OrderByDescending(mov => mov.fechaMovimiento).FirstOrDefault().balance;
                }else return 0;
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
        internal int ajustarBalances(MovimientoFinanciero ultimoMovimiento, ApplicationDbContext db)
        {
            /*Tomando como referencia el ultimo movimiento anterior al recien modificado, se toman
            todos los registros posteriores a este, en caso de que el recien modificado sea el 1ro,
            se toman por defecto todos los registros existentes*/
            var movimientos = ultimoMovimiento!=null ? this.MovimientosFinancieros
                    .Where(mov => mov.fechaMovimiento > ultimoMovimiento.fechaMovimiento)
                    : this.MovimientosFinancieros;

            //Se crea una lista encadenada ordenada cronologicamente hacia el pasado
            movimientos = movimientos.OrderByDescending(mov => mov.fechaMovimiento).ToList();
            LinkedList<MovimientoFinanciero> movimientosOrdenados = new LinkedList<MovimientoFinanciero>(movimientos);

            //Para el registro recien modificado, se recalcula su balance
            movimientosOrdenados.Last.Value.balance = movimientosOrdenados.Last.Value.montoMovimiento + 
                (ultimoMovimiento==null ? 0 : ultimoMovimiento.balance);

            /*Recorriendo la lista encadenada desde el registro recien modificado hasta el ultimo
            se vam corrigiendo los balances, uno tras otro*/
            int numreg = 0;
            if (movimientos.Count() > 1)
            {
                //apuntador actual
                var nodePointer = movimientosOrdenados.Last;
                while (nodePointer.Previous != null)
                {
                    //nodo anterior
                    var nodo = nodePointer.Previous;

                    //Se calcula nuevo balance
                    nodo.Value.balance = nodePointer.Value.balance + nodo.Value.montoMovimiento;

                    //Se recorre apuntador
                    nodePointer = nodo;
                }
            }

            //Se notifica la edicion de los registros modificados
            foreach (var mov in movimientosOrdenados)
                db.Entry(mov).State = System.Data.Entity.EntityState.Modified;

            //Se guardan cambios
            numreg = db.SaveChanges();

            return numreg;
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
                .OrderByDescending(mov => mov.fechaMovimiento).FirstOrDefault();

            return m;
        }

        internal MovimientoFinanciero getUltimoMovimiento(DateTime fechaMovimiento)
        {
            /*
            var movs = this.MovimientosFinancieros
                .Where(mov => mov.fechaMovimiento <= fechaMovimiento).ToList()
                .OrderByDescending(mov => mov.fechaMovimiento);

            MovimientoFinanciero m = movs.FirstOrDefault();
            */

            var movs = this.MovimientosFinancieros
                .Where(mov => mov.fechaMovimiento <= fechaMovimiento)
                .OrderByDescending(mov => mov.fechaMovimiento)
                .Take(2).ToList();

            MovimientoFinanciero m;
            if (movs.Count() > 0)
                m = null;
            else
                m = movs.ElementAt(movs.Count() - 1);

            return m;
        }
    }
}