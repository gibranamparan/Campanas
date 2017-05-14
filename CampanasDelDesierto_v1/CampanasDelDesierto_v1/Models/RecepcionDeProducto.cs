using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace CampanasDelDesierto_v1.Models
{
    public class RecepcionDeProducto
    {
        public enum ExcelCols{
            NUM_RECIBO=0,NOMBRE_PRODUCTOR=6,PROD_NOMBRE=8,
            TONS_MANZANA=10, TONS_OBLISSA=11, TONS_MISION=12
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
        public double cantidadTonsProd1 { get; set; }

        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO2)]
        public double cantidadTonsProd2 { get; set; }

        [DisplayName(TemporadaDeCosecha.TiposDeProducto.PRODUCTO3)]
        public double cantidadTonsProd3 { get; set; }

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
        public RecepcionDeProducto(ExcelRange excelRange, int temporadaID)
        {
            int rowNum = excelRange.Start.Row;
            this.numeroRecibo = int.Parse(excelRange.ElementAt((int)ExcelCols.NUM_RECIBO).Value.ToString());
            this.numProductor = excelRange.ElementAt((int)ExcelCols.NOMBRE_PRODUCTOR).Value.ToString();
            this.nombreProductor = excelRange.ElementAt((int)ExcelCols.PROD_NOMBRE).Value.ToString();
            this.cantidadTonsProd1 = double.Parse(excelRange.ElementAt((int)ExcelCols.TONS_MANZANA).Value.ToString());
            this.cantidadTonsProd2 = double.Parse(excelRange.ElementAt((int)ExcelCols.TONS_OBLISSA).Value.ToString());
            this.cantidadTonsProd3 = double.Parse(excelRange.ElementAt((int)ExcelCols.TONS_MISION).Value.ToString());
            this.TemporadaDeCosechaID = temporadaID;
        }
    }
}