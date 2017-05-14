using CampanasDelDesierto_v1.HerramientasGenerales;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
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

        [Display(Name ="Rango de Tiempo")]
        public string rangoTiempo { get {
                System.Globalization.CultureInfo cult = System.Globalization
                    .CultureInfo.CreateSpecificCulture("es-MX");
                string salida = this.fechaInicio.ToString("dd/MMMM/yy", cult);
                salida+=" - "+ this.fechaFin.ToString("dd/MMMM/yy", cult);
                return salida;
            } }

        //Tipos de producto
        public string tipoProducto1 { get { return TiposDeProducto.PRODUCTO1; } }
        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        public decimal precioProducto1 { get; set; }

        public string tipoProducto2 { get { return TiposDeProducto.PRODUCTO2; } }
        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        public decimal precioProducto2 { get; set; }

        public string tipoProducto3 { get { return TiposDeProducto.PRODUCTO3; } }
        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        public decimal precioProducto3 { get; set; }

        public string tipoProductoOtro { get { return TiposDeProducto.OTRO; } }
        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        public decimal precioProductoOtro { get; set; }

        //Los movimientos financieros se registran dentro de una temporada de cosecha
        public virtual ICollection<MovimientoFinanciero> movimientosFinancieros { get; set; }

        //Los movimientos financieros se registran dentro de una temporada de cosecha
        public virtual ICollection<RecepcionDeProducto> recepcionesDeProducto { get; set; }

        internal int importarIngresoDeProductos(HttpPostedFileBase xlsFile,ApplicationDbContext db)
        {
            int regsSaved = 0;
            if ((xlsFile != null) && (xlsFile.ContentLength > 0) && !string.IsNullOrEmpty(xlsFile.FileName))
            {
                string fileName = xlsFile.FileName;
                string fileContentType = xlsFile.ContentType;
                byte[] fileBytes = new byte[xlsFile.ContentLength];
                var data = xlsFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(xlsFile.ContentLength));

                if (this.recepcionesDeProducto == null)
                    this.recepcionesDeProducto = new List<RecepcionDeProducto>();

                var package = new ExcelPackage(xlsFile.InputStream);
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;

                for (int rowIterator = 5; rowIterator <= 15; rowIterator++)
                {
                    var rowRecepcion = workSheet.Cells[rowIterator, 1, rowIterator, noOfCol];
                    var recepcion = new RecepcionDeProducto(rowRecepcion, this.TemporadaDeCosechaID);
                    var recepcionDB = this.recepcionesDeProducto.ToList()
                        .FirstOrDefault(rp => rp.numeroRecibo == recepcion.numeroRecibo);
                    //Si el registro no existe, se agrega
                    if (recepcionDB == null)
                        db.RecepcionesDeProducto.Add(recepcion);
                    else {
                        //Si ya existe, se identifica con el mismo ID y se marca como modificado
                        recepcion.recepcionID = recepcionDB.recepcionID;
                        //db.Entry(recepcion).State = System.Data.Entity.EntityState.Modified;
                        this.recepcionesDeProducto.Remove(recepcionDB);
                        this.recepcionesDeProducto.Add(recepcion);
                    }
                }
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
            public const string OTRO = "OTRO";
        }

        public VMTipoProducto[] getListaProductos()
        {
            VMTipoProducto[] opciones = new VMTipoProducto[]{
                new VMTipoProducto(this.precioProducto1, this.tipoProducto1),
                new VMTipoProducto(this.precioProducto2, this.tipoProducto2),
                new VMTipoProducto(this.precioProducto3, this.tipoProducto3),
                new VMTipoProducto(this.precioProductoOtro, this.tipoProductoOtro),
            };
            return opciones;
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

    }
}