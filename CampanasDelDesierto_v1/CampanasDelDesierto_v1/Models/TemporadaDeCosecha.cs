using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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


        private int DIA_INICIO_PERIODO = 30;
        private int DIA_FIN_PERIODO = 30;
        private int MES_PERIODO = 8;

        public TemporadaDeCosecha()
        {
            int anioActual = DateTime.Today.Year;
            this.fechaInicio = new DateTime(anioActual, MES_PERIODO, DIA_INICIO_PERIODO);
            this.fechaFin = this.fechaInicio.AddYears(1);
        }

        public static class TiposDeProducto
        {
            public static readonly string PRODUCTO1 = "MANZANITA";
            public static readonly string PRODUCTO2 = "OBLIZA";
            public static readonly string PRODUCTO3 = "MISSION";
            public static readonly string OTRO = "OTRO";
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