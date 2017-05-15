using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExcelNums = CampanasDelDesierto_v1.Models.RecepcionDeProducto.ExcelCols;

namespace CampanasDelDesierto_v1.Models
{
    public class RecepcionDeProducto
    {
        public enum ExcelCols{
            NUM_RECIBO=0, DIA = 4, SEMANA = 5,
            NOMBRE_PRODUCTOR =6,PROD_NOMBRE=8,
            TONS_MANZANA=10, TONS_OBLISSA=11, TONS_MISION=12,
            ROW_COSTOS = 2, PRECIO_PROD1 = 12, PRECIO_PROD2 = 13,
            PRECIO_PROD3 = 14, PRECIO_PROD_OTRO = 15,
        }

        [Key]
        public int recepcionID { get; set; }

        [DisplayName("# Recibo")]
        public int numeroRecibo { get; set; }

        [DisplayName("# Productor")]
        public string numProductor { get; set; }

        [DisplayName("Nombre de Productor")]
        public string nombreProductor { get; set; }

        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO1)]
        [DisplayFormat(DataFormatString ="{0:0.000}")]
        public double cantidadTonsProd1 { get; set; }

        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO2)]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public double cantidadTonsProd2 { get; set; }

        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO3)]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public double cantidadTonsProd3 { get; set; }

        [DisplayName("Fecha")]
        public DateTime fecha { get; set; }

        [DisplayName("Semana")]
        public int semana { get; set; }

        [DisplayName("Temporada")]
        public int? TemporadaDeCosechaID { get; set; }
        public TemporadaDeCosecha temporada { get; set; }

        [DisplayName("Productor")]
        public int? idProductor { get; set; }
        public Productor productor { get; set; }

        public RecepcionDeProducto() { }
        /// <summary>
        /// Crea un registro de ingreso de producto basado de la importación de un excel.
        /// </summary>
        /// <param name="excelRange">Rango de excel que representa un renglon.</param>
        /// <param name="temporadaID">Temporada a la que corresponde el ingreso</param>
        public RecepcionDeProducto(ExcelRange excelRange, int temporadaID, ref VMRecepcionProductoError error)
        {
            error = new VMRecepcionProductoError();
            int rowNum = excelRange.Start.Row;
            try
            {
                this.numeroRecibo = int.Parse(excelRange.ElementAt((int)ExcelCols.NUM_RECIBO).Value.ToString());
                this.fecha = DateTime.Parse(excelRange.ElementAt((int)ExcelCols.DIA).Value.ToString());
                this.semana = int.Parse(excelRange.ElementAt((int)ExcelCols.SEMANA).Value.ToString());
                this.numeroRecibo = int.Parse(excelRange.ElementAt((int)ExcelCols.NUM_RECIBO).Value.ToString());
                this.numProductor = excelRange.ElementAt((int)ExcelCols.NOMBRE_PRODUCTOR).Value.ToString();
                this.nombreProductor = excelRange.ElementAt((int)ExcelCols.PROD_NOMBRE).Value.ToString();
                this.cantidadTonsProd1 = double.Parse(excelRange.ElementAt((int)ExcelCols.TONS_MANZANA).Value.ToString());
                this.cantidadTonsProd2 = double.Parse(excelRange.ElementAt((int)ExcelCols.TONS_OBLISSA).Value.ToString());
                this.cantidadTonsProd3 = double.Parse(excelRange.ElementAt((int)ExcelCols.TONS_MISION).Value.ToString());
                this.TemporadaDeCosechaID = temporadaID;
                error.registro = this;
            }
            catch (NullReferenceException exc)
            {
                error = errorFromException(exc, excelRange);
            }
            catch (FormatException exc)
            {
                error = errorFromException(exc, excelRange);
            }
            catch (Exception exc)
            {
                error = errorFromException(exc, excelRange);
            }
        }

        public RecepcionDeProducto(RecepcionDeProducto otro)
        {
            this.numeroRecibo = otro.numeroRecibo;
            this.numProductor = otro.numProductor;
            this.nombreProductor = otro.nombreProductor;
        }

        public VMRecepcionProductoError errorFromException(Exception exc, ExcelRange excelRang)
        {
            VMRecepcionProductoError error = new VMRecepcionProductoError();
            error.isError = true;
            error.errorDetails = exc.Message;
            error.registro = new RecepcionDeProducto(this);
            string errorMsg = "";
            if (exc is NullReferenceException)
                errorMsg = "No contiene información.";
            else if (exc is FormatException)
                errorMsg = "Contiene un dato que no es posible transformar.";
            else if(exc is Exception)
                errorMsg = "Error inesperado, favor de ver los detalles.";

            error.errorMsg = String.Format("Renglon : <strong>{0}</strong>. Error: {1}", excelRang.Address, errorMsg);

            return error;
        }

        public class VMRecepcionProductoError
        {
            public RecepcionDeProducto registro { get; set; }
            [DisplayName("Error")]
            public string errorMsg { get; set; }
            public bool isError = false;
            public string errorDetails;

            public override string ToString()
            {
                return this.errorMsg;
            }
        }

        public class VMCostosDeProducto {
            public string nombre { get; set; }
            public decimal costo { get; set; }
            public VMCostosDeProducto() { }
            public VMCostosDeProducto(ExcelRange excelRange)
            {
                this.nombre = excelRange.ElementAt(0).Value.ToString();
                this.costo = decimal.Parse(excelRange.ElementAt(1).Value.ToString());
            }
            public VMCostosDeProducto(string nombre, decimal costo)
            {
                this.nombre = nombre;
                this.costo = costo;
            }

            internal static TemporadaDeCosecha tomarCostosProducto(ref ExcelWorksheet workSheet, ref VMRecepcionProductoError error)
            {
                error = new VMRecepcionProductoError();
                TemporadaDeCosecha tc = new TemporadaDeCosecha();

                int rowNum = (int)ExcelNums.ROW_COSTOS;
                int inicio = (int)ExcelNums.PRECIO_PROD1;
                int fin = (int)ExcelNums.PRECIO_PROD_OTRO;
                var rowNombre = workSheet.Cells[rowNum-1, inicio, rowNum-1, fin];
                var rowCostos = workSheet.Cells[rowNum, inicio, rowNum, fin];

                try { 
                    VMCostosDeProducto p1 = new VMCostosDeProducto(workSheet.Cells[rowNum - 1,
                        (int)ExcelNums.PRECIO_PROD1, rowNum, (int)ExcelNums.PRECIO_PROD1]);
                    VMCostosDeProducto p2 = new VMCostosDeProducto(workSheet.Cells[rowNum - 1,
                        (int)ExcelNums.PRECIO_PROD2, rowNum, (int)ExcelNums.PRECIO_PROD2]);
                    VMCostosDeProducto p3 = new VMCostosDeProducto(workSheet.Cells[rowNum - 1,
                        (int)ExcelNums.PRECIO_PROD3, rowNum, (int)ExcelNums.PRECIO_PROD3]);
                    VMCostosDeProducto p4 = new VMCostosDeProducto(workSheet.Cells[rowNum - 1,
                        (int)ExcelNums.PRECIO_PROD_OTRO, rowNum, (int)ExcelNums.PRECIO_PROD_OTRO]);
                    tc.precioProducto1 = p1.costo;
                    tc.precioProducto2 = p2.costo;
                    tc.precioProducto3 = p3.costo;
                    tc.precioProductoOtro = p4.costo;
                }catch(Exception exc)
                {
                    error.errorMsg = "Se presentaron problemas al tomar los costos del producto";
                    error.errorDetails = exc.Message;
                    error.isError = true;
                }

                return tc;
            }
        }
    }
}