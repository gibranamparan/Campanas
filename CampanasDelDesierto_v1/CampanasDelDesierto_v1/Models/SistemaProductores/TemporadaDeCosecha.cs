using CampanasDelDesierto_v1.HerramientasGenerales;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class TemporadaDeCosecha
    {
        [Key]
        public int TemporadaDeCosechaID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name ="Inicio")]
        public DateTime fechaInicio { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fin")]
        public DateTime fechaFin { get; set; }

        public TimePeriod periodo { get { return new TimePeriod(this.fechaInicio, this.fechaFin); } }

        [Display(Name ="Temporada de Cosecha")]
        public string rangoTiempo { get {
                System.Globalization.CultureInfo cult = System.Globalization
                    .CultureInfo.CreateSpecificCulture("es-MX");
                string salida = this.fechaInicio.ToString("dd/MMMM/yy", cult);
                salida+=" - "+ this.fechaFin.ToString("dd/MMMM/yy", cult);
                return salida;
            } }

        //Tipos de producto
        /*MANZANITA CABORCA*/
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO1 + "(ton.)")]
        public string tipoProducto1 { get { return TiposDeProducto.PRODUCTO1; } }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO1)]
        public decimal precioProducto1 { get; set; }

        /*OBLIZA CABORCA*/
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO2+ "(ton.)")]
        public string tipoProducto2 { get { return TiposDeProducto.PRODUCTO2; } }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO2)]
        public decimal precioProducto2 { get; set; }

        /*MISSION CABORCA*/
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO3 + "(ton.)")]
        public string tipoProducto3 { get { return TiposDeProducto.PRODUCTO3; } }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO3)]
        public decimal precioProducto3 { get; set; }

        /*MANZANITA BAJA*/
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO1 + " BAJA (ton.)")]
        public string tipoProducto4 { get { return TiposDeProducto.PRODUCTO1 + " BAJA"; } }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO1 + " BAJA")]
        public decimal precioProducto4 { get; set; }

        /*OBLIZA BAJA*/
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO2 + " BAJA (ton.)")]
        public string tipoProducto5 { get { return TiposDeProducto.PRODUCTO2 + " BAJA"; } }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO2 + " BAJA")]
        public decimal precioProducto5 { get; set; }


        /*MISSION BAJA*/
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO3 + " BAJA (ton.)")]
        public string tipoProducto6 { get { return TiposDeProducto.PRODUCTO3 + " BAJA"; } }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO3 + " BAJA")]
        public decimal precioProducto6 { get; set; }


        //Los movimientos financieros se registran dentro de una temporada de cosecha
        public virtual ICollection<MovimientoFinanciero> movimientosFinancieros { get; set; }

        //Los movimientos financieros se registran dentro de una temporada de cosecha
        public virtual ICollection<RecepcionDeProducto> recepcionesDeProducto { get; set; }

        /// <summary>
        /// Almancena en base de datos todos los registros de ingreso de productos 
        /// capturados en un archivo Excel dado.
        /// </summary>
        /// <param name="xlsFile">Archivo excel proveniente de una forma desde el explorador.</param>
        /// <param name="db">Contexto de la base de datos.</param>
        /// <param name="errores">Lista de errores donde sobre la cual se iran almacenando los 
        /// errores de interpretación de datos en caso de suceder.</param>
        /// <param name="errorPrecios">Reporta errores generales de la importación.</param>
        /// <returns></returns>
        public int importarIngresoDeProductos(HttpPostedFileBase xlsFile,ApplicationDbContext db,
            out List<ExcelTools.ExcelParseError> errores, out ExcelTools.ExcelParseError errorPrecios)
        {
            int regsSaved = 0;
            //Lista para recoleccion de errores
            errores = new List<ExcelTools.ExcelParseError>();
            errorPrecios = new ExcelTools.ExcelParseError();
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
                var workSheet = package.Workbook.Worksheets["Datos"];//Se toma la 1ra hoja de excel
                var noOfCol = workSheet.Dimension.End.Column;//Se determina el ancho de la tabla en no. columnas
                var noOfRow = workSheet.Dimension.End.Row;//El alto de la tabla en numero de renglores

                ExcelTools.ExcelParseError error = new ExcelTools.ExcelParseError();
                //Se recorre cada renglon de la hoja
                //for (int rowIterator = 5; rowIterator <= noOfRow; rowIterator++)
                for (int rowIterator = 5; rowIterator <= noOfRow; rowIterator++)
                {
                    //Se toma renglon
                    var rowRecepcion = workSheet.Cells[rowIterator, 1, rowIterator, noOfCol];
                    //Se convierte renglon en registro para DB con registro de errores
                    var recepcion = new RecepcionDeProducto(rowRecepcion, this.TemporadaDeCosechaID,ref error);

                    //Si la importacion contiene todos sus elementos validos
                    if (!error.isError) { 
                        var recepcionDB = this.recepcionesDeProducto.ToList()
                            .FirstOrDefault(rp => rp.numeroRecibo == recepcion.numeroRecibo);
                        //Si el registro no existe, se agrega
                        if (recepcionDB == null)
                            db.RecepcionesDeProducto.Add(recepcion);
                        else {
                            //Si ya existe, se identifica con el mismo ID y se marca como modificado
                            recepcion.recepcionID = recepcionDB.recepcionID;

                            db.RecepcionesDeProducto.Remove(recepcionDB);
                            //Se deja de trackear el registro existente para poder ser modificado
                            db.Entry(recepcionDB).State = System.Data.Entity.EntityState.Detached;
                            //Se marca como modificado
                            db.Entry(recepcion).State = System.Data.Entity.EntityState.Modified;
                        }
                    }else //Si existe algun error en la importacion de la columna
                        errores.Add(error);
                }

                //Se toman los costos por tonelada de producto
                TemporadaDeCosecha tc = RecepcionDeProducto.VMCostosDeProducto.tomarCostosProducto(ref workSheet, ref error);
                if (!error.isError) { 
                    this.precioProducto1 = tc.precioProducto1;
                    this.precioProducto2 = tc.precioProducto2;
                    this.precioProducto3 = tc.precioProducto3;
                }
                errorPrecios = error;

                db.Entry(this).State = System.Data.Entity.EntityState.Modified;
                regsSaved = db.SaveChanges();
            }
            return regsSaved;
        }

        private int DIA_INICIO_PERIODO = 30;
        private int DIA_FIN_PERIODO = 30;
        private int MES_PERIODO = 8;

        public static TemporadaDeCosecha findTemporada(int? temporada)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            TemporadaDeCosecha tem;
            if (temporada == null || temporada.Value == 0)
                tem = getUltimaTemporada();
            else
                tem = db.TemporadaDeCosechas.Find(temporada);
            if (tem == null)
                tem = getUltimaTemporada();

            return tem;
        }

        public TemporadaDeCosecha()
        {
            int anioActual = DateTime.Today.Year;
            this.fechaInicio = new DateTime(anioActual, MES_PERIODO, DIA_INICIO_PERIODO);
            this.fechaFin = this.fechaInicio.AddYears(1);
        }

        public static TemporadaDeCosecha getUltimaTemporada()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return getUltimaTemporada(db);
        }

        public static TemporadaDeCosecha getUltimaTemporada(ApplicationDbContext db)
        {
            var tem = db.TemporadaDeCosechas.OrderByDescending(t => t.fechaInicio).FirstOrDefault();
            return tem;
        }

        public static class TiposDeProducto
        {
            public const string PRODUCTO1 = "MANZANITA";
            public const string PRODUCTO2 = "OBLIZA";
            public const string PRODUCTO3 = "MISSION";
            public const string OTRO = "MISSION-BAJA";
        }

        public VMTipoProducto[] getListaProductos()
        {
            VMTipoProducto[] opciones = new VMTipoProducto[]{
                new VMTipoProducto(this.precioProducto1, this.tipoProducto1),
                new VMTipoProducto(this.precioProducto2, this.tipoProducto2),
                new VMTipoProducto(this.precioProducto3, this.tipoProducto3),
            };
            return opciones;
        }

        public override string ToString()
        {
            return this.periodo.ToString();
        }

        public class VMTipoProducto
        {
            public decimal Value { get; set; }
            public string Text { get; set; }
            public VMTipoProducto() { }
            public VMTipoProducto(decimal Value, string Text)
            {
                this.Value = Value;
                this.Text = Text;
            }
        }

        public class VMPreciosProductos
        {
            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO1)]
            public decimal precioManzanita { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO2)]
            public decimal precioObliza { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO3)]
            public decimal precioMission { get; set; }

            [DisplayName("Zona")]
            public string zona { get; set; }

            public TemporadaDeCosecha temporada { get; set; }

            public VMPreciosProductos() { this.zona = Productor.Zonas.ZONA1; }
            public VMPreciosProductos(TemporadaDeCosecha temporada, string zona)
            {
                if(zona == Productor.Zonas.ZONA1)
                {
                    this.precioManzanita = temporada.precioProducto1;
                    this.precioObliza= temporada.precioProducto2;
                    this.precioMission = temporada.precioProducto3;
                }
                else if(zona == Productor.Zonas.ZONA2)
                {
                    this.precioManzanita = temporada.precioProducto4;
                    this.precioObliza = temporada.precioProducto5;
                    this.precioMission = temporada.precioProducto6;
                }
                this.zona = zona;
                this.temporada = temporada;
            }
        }
    }
}