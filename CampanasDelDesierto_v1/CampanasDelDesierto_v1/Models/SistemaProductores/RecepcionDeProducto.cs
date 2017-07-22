using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExcelNums = CampanasDelDesierto_v1.Models.RecepcionDeProducto.ExcelCols;
using CampanasDelDesierto_v1.HerramientasGenerales;

namespace CampanasDelDesierto_v1.Models
{
    public class RecepcionDeProducto
    {
        public enum ExcelCols {
            //Datos del registro de cosecha
            NUM_RECIBO = 0, DIA = 7, SEMANA = 8,
            NUM_PRODUCTOR = 9, PROD_NOMBRE = 11,
            ROW_COSTOS = 2,
            //Toneladas de producto
            TONS_MANZANA = 13, TONS_OBLISA = 14, TONS_MISSION = 15,
            TONS_MANZANA_ORG = 16, TONS_OBLIZA_ORG = 17, TONS_MISSION_ORG = 18,
            //Precios
            PRECIO_PROD_MANZANA = 14, PRECIO_PROD_OBLIZA = 15,
            PRECIO_PROD_MISSION = 16, PRECIO_PROD_MISSION_BAJA = 17,
            //Precios Organicos
            PRECIO_PROD_MANZANA_ORG = 19, PRECIO_PROD_OBLIZA_ORG = 20,
            PRECIO_PROD_MISSION_ORG = 21, PRECIO_PROD_MISSION_BAJA_ORG = 22
        }

        [Key]
        public int recepcionID { get; set; }

        [DisplayName("Num. Recibo")]
        [Range(1,int.MaxValue)]
        [Required]
        public int numeroRecibo { get; set; }

        [DisplayName("Num. Productor")]
        public string numProductor { get; set; }

        [DisplayName("Nombre de Productor")]
        public string nombreProductor { get; set; }

        //Manzana Caborca
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO1 + " (ton.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public double cantidadTonsProd1 { get; set; }

        //Obliza Caborca
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO2 + " (ton.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public double cantidadTonsProd2 { get; set; }

        //Mission Caborca
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO3 + " (ton.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public double cantidadTonsProd3 { get; set; }

        //Manzana Organica
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO4 + " (ton.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public double cantidadTonsProd4 { get; set; }

        //Obliza Organica
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO5 + " (ton.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public double cantidadTonsProd5 { get; set; }

        //Mission Organica
        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO6 + " (ton.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public double cantidadTonsProd6 { get; set; }

        [Required]
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
            ApplyFormatInEditMode = true)]
        public DateTime fecha { get; set; }

        [DisplayName("Semana")]
        public int semana { get; set; }

        [DisplayName("Temporada")]
        public int? TemporadaDeCosechaID { get; set; }
        public virtual TemporadaDeCosecha temporada { get; set; }

        [DisplayName("Productor")]
        public int? idProductor { get; set; }
        public virtual Productor productor { get; set; }

        [DisplayName("Pago")]
        [ForeignKey("pago")]
        public int? movimientoID { get; set; }
        public virtual PagoPorProducto pago { get; set;}

        [DisplayName("Importado desde Excel")]
        public bool importadoDesdeExcel { get; set; }

        public bool pagoYaRegistrado
        {
            get
            {
                return this.movimientoID != null && this.pago != null;
            }
        }

        public bool liquidacionYaEmitada
        {
            get
            {
                return this.pagoYaRegistrado && this.pago.liquidacionDeCosechaID != null 
                    && this.pago.liquidacionDeCosecha != null;
            }
        }

        public RecepcionDeProducto() { }
        /// <summary>
        /// Crea un registro de ingreso de producto basado de la importación de un excel.
        /// </summary>
        /// <param name="excelRange">Rango de excel que representa un renglon.</param>
        /// <param name="temporadaID">Temporada a la que corresponde el ingreso</param>
        public RecepcionDeProducto(ExcelRange excelRange, int temporadaID, ref ExcelTools.ExcelParseError error)
        {
            error = new ExcelTools.ExcelParseError();
            int rowNum = excelRange.Start.Row;
            try
            {
                this.numeroRecibo = int.Parse(excelRange.ElementAt((int)ExcelCols.NUM_RECIBO).Value.ToString().Trim());
                this.fecha = DateTime.Parse(excelRange.ElementAt((int)ExcelCols.DIA).Value.ToString());
                this.semana = int.Parse(excelRange.ElementAt((int)ExcelCols.SEMANA).Value.ToString());
                this.numProductor = excelRange.ElementAt((int)ExcelCols.NUM_PRODUCTOR).Value.ToString();
                this.nombreProductor = excelRange.ElementAt((int)ExcelCols.PROD_NOMBRE).Value.ToString().Trim();
                this.cantidadTonsProd1 = double.Parse(excelRange.ElementAt((int)ExcelCols.TONS_MANZANA).Value.ToString());
                this.cantidadTonsProd2 = double.Parse(excelRange.ElementAt((int)ExcelCols.TONS_OBLISA).Value.ToString());
                this.cantidadTonsProd3 = double.Parse(excelRange.ElementAt((int)ExcelCols.TONS_MISSION).Value.ToString());
                this.cantidadTonsProd4 = double.Parse(excelRange.ElementAt((int)ExcelCols.TONS_MANZANA_ORG).Value.ToString());
                this.cantidadTonsProd5 = double.Parse(excelRange.ElementAt((int)ExcelCols.TONS_OBLIZA_ORG).Value.ToString());
                this.cantidadTonsProd6 = double.Parse(excelRange.ElementAt((int)ExcelCols.TONS_MISSION_ORG).Value.ToString());
                this.TemporadaDeCosechaID = temporadaID;
                this.importadoDesdeExcel = true;
                error.registro = this;
            }
            catch (NullReferenceException exc) //Error con celdas vacias
            {
                error = ExcelTools.ExcelParseError.errorFromException(exc, excelRange);
                error.registro = new RecepcionDeProducto(this);
            }
            catch (FormatException exc) //Error con celdas numericas no transformables a numeros
            {
                error = ExcelTools.ExcelParseError.errorFromException(exc, excelRange);
                error.registro = new RecepcionDeProducto(this);
            }
            catch (Exception exc) //Errores desconocidos
            {
                error = ExcelTools.ExcelParseError.errorFromException(exc, excelRange);
                error.registro = new RecepcionDeProducto(this);
            }
        }

        public RecepcionDeProducto(RecepcionDeProducto otro)
        {
            this.numeroRecibo = otro.numeroRecibo;
            this.numProductor = otro.numProductor;
            this.nombreProductor = otro.nombreProductor;
        }

        public override string ToString()
        {
            return String.Format("#Recibo: {0} - {1}: {2}",this.numeroRecibo, this.numProductor, this.nombreProductor);
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

            internal static TemporadaDeCosecha tomarCostosProducto(ref ExcelWorksheet workSheet, ref ExcelTools.ExcelParseError error)
            {
                error = new ExcelTools.ExcelParseError();
                TemporadaDeCosecha tc = new TemporadaDeCosecha();

                int rowNum = (int)ExcelNums.ROW_COSTOS;
                int inicio = (int)ExcelNums.PRECIO_PROD_MANZANA;
                int fin = (int)ExcelNums.PRECIO_PROD_MISSION_ORG;
                var rowNombre = workSheet.Cells[rowNum-1, inicio, rowNum-1, fin];
                var rowCostos = workSheet.Cells[rowNum, inicio, rowNum, fin];

                try { 
                    VMCostosDeProducto pManzana = new VMCostosDeProducto(workSheet.Cells[rowNum - 1,
                        (int)ExcelNums.PRECIO_PROD_MANZANA, rowNum, (int)ExcelNums.PRECIO_PROD_MANZANA]);
                    VMCostosDeProducto pObliza = new VMCostosDeProducto(workSheet.Cells[rowNum - 1,
                        (int)ExcelNums.PRECIO_PROD_OBLIZA, rowNum, (int)ExcelNums.PRECIO_PROD_OBLIZA]);
                    VMCostosDeProducto pMission = new VMCostosDeProducto(workSheet.Cells[rowNum - 1,
                        (int)ExcelNums.PRECIO_PROD_MISSION, rowNum, (int)ExcelNums.PRECIO_PROD_MISSION]);
                    VMCostosDeProducto pMissionBaja = new VMCostosDeProducto(workSheet.Cells[rowNum - 1,
                        (int)ExcelNums.PRECIO_PROD_MISSION_BAJA, rowNum, (int)ExcelNums.PRECIO_PROD_MISSION_BAJA]);
                    VMCostosDeProducto pManzanaOrg = new VMCostosDeProducto(workSheet.Cells[rowNum - 1,
                        (int)ExcelNums.PRECIO_PROD_MANZANA_ORG, rowNum, (int)ExcelNums.PRECIO_PROD_MANZANA_ORG]);
                    VMCostosDeProducto pOblizaOrg = new VMCostosDeProducto(workSheet.Cells[rowNum - 1,
                        (int)ExcelNums.PRECIO_PROD_OBLIZA_ORG, rowNum, (int)ExcelNums.PRECIO_PROD_OBLIZA_ORG]);
                    VMCostosDeProducto pMissionOrg = new VMCostosDeProducto(workSheet.Cells[rowNum - 1,
                        (int)ExcelNums.PRECIO_PROD_MISSION_ORG, rowNum, (int)ExcelNums.PRECIO_PROD_MISSION_ORG]);
                    VMCostosDeProducto pMissionOrgBaja = new VMCostosDeProducto(workSheet.Cells[rowNum - 1,
                        (int)ExcelNums.PRECIO_PROD_MISSION_BAJA_ORG, rowNum, (int)ExcelNums.PRECIO_PROD_MISSION_BAJA_ORG]);
                    tc.precioProducto1 = pManzana.costo; //manzana
                    tc.precioProducto2 = pObliza.costo; //obliza
                    tc.precioProducto3 = pMission.costo; //mission
                    tc.precioProducto6 = pMissionBaja.costo; //mission baja
                    tc.precioProducto7 = pManzanaOrg.costo; //manzana organica
                    tc.precioProducto9 = pOblizaOrg.costo; //obliza organica
                    tc.precioProducto11 = pMissionOrg.costo; //mission organica
                    tc.precioProducto12 = pMissionOrgBaja.costo; //mission baja organica
                }
                catch(Exception exc)
                {
                    error.errorMsg = "Se presentaron problemas al tomar los costos del producto";
                    error.errorDetails = exc.Message;
                    error.isError = true;
                }

                return tc;
            }
        }

        /// <summary>
        /// Representa un renglon dentro del reporte de entregas totales en la semana.
        /// Es utilizado para el reporte de liquidacion semanal.
        /// </summary>
        public class VMTotalDeEntregas
        {
            /// <summary>
            /// Nombre estandar de la variedad del producto. 
            /// </summary>
            [DisplayName("Variedad")]
            public string producto { get; set; }
            /// <summary>
            /// Precio unitario por tonelada correspondente a la variedad de producto.
            /// Esto definido dentro de la temporada.
            /// </summary>
            [DisplayFormat(DataFormatString = "{0:C}", 
                ApplyFormatInEditMode = true)]
            [DisplayName("Precio Tonelada")]
            public decimal precio { get; set; }
            /// <summary>
            /// Toneladas totales correspondientes a la variedad, recibidas en la semana
            /// </summary>
            [DisplayFormat(DataFormatString = "{0:0.000}")]
            [DisplayName("Toneladas Entregadas")]
            public double toneladasRecibidas { get; set; }
            /// <summary>
            /// Precio total de las toneladas recibidas
            /// </summary>
            [DisplayFormat(DataFormatString = "{0:C}",
                ApplyFormatInEditMode = true)]
            [DisplayName("Monto (USD)")]
            public decimal monto { get; set; }
            /// <summary>
            /// Mismo monto en pesos mexicanos segun el tipo de cambio dado al preparar el reporte.
            /// </summary>
            [DisplayFormat(DataFormatString = "{0:C}",
                ApplyFormatInEditMode = true)]
            [DisplayName("Monto (MXN)")]
            public decimal montoMXN { get; set; }
        }

        public class VMRecepcionProducto{
            public int recepcionID { get; set; }

            [DisplayName("# Recibo")]
            public int numeroRecibo { get; set; }
            
            [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO1 + " (ton.)")]
            [DisplayFormat(DataFormatString = "{0:0.000}")]
            public double cantidadTonsProd1 { get; set; }

            [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO2 + " (ton.)")]
            [DisplayFormat(DataFormatString = "{0:0.000}")]
            public double cantidadTonsProd2 { get; set; }

            [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO3 + " (ton.)")]
            [DisplayFormat(DataFormatString = "{0:0.000}")]
            public double cantidadTonsProd3 { get; set; }

            [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO4 + " (ton.)")]
            [DisplayFormat(DataFormatString = "{0:0.000}")]
            public double cantidadTonsProd4 { get; set; }

            [DisplayName("Fecha")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "yyyy-MM-dd",
                ApplyFormatInEditMode = true)]
            public DateTime fecha { get; set; }

            [DisplayName("Semana")]
            public int semana { get; set; }

            public VMRecepcionProducto() { }
            public VMRecepcionProducto(RecepcionDeProducto rdp)
            {
                this.numeroRecibo = rdp.numeroRecibo;
                this.cantidadTonsProd1 = rdp.cantidadTonsProd1;
                this.cantidadTonsProd2 = rdp.cantidadTonsProd2;
                this.cantidadTonsProd3 = rdp.cantidadTonsProd3;
                this.cantidadTonsProd4 = rdp.cantidadTonsProd4;
                this.fecha = rdp.fecha;
                this.semana = rdp.semana;
            }
        }
    }
}