using CampanasDelDesierto_v1.HerramientasGenerales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CampanasDelDesierto_v1.Models
{
    public class PrestamoYAbonoCapital:MovimientoFinanciero
    {
        [Display(Name = "Cheque/Folio")]
        public string cheque { get; set; }

        [Display(Name = "Tipo de Movimiento")]
        public string tipoDeMovimientoDeCapital { get; set; }

        //public double cargo { get; set; }
        [Display(Name = "Pagaré")]
        public string pagare { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha a pagar")]
        public DateTime? fechaPagar { get; set; }

        [Display(Name = "Proveedor")]
        public string proveedor { get; set; }
        
        [Display(Name = "Concepto")]
        public string concepto { get; set; }

        [Display(Name = "Descripcion de Concepto")]
        [DataType(DataType.MultilineText)]
        public string descripcionConcepto { get; set; }

        [Display(Name = "Pozo")]
        public string pozo { get; set; }

        [Display(Name = "Precio del dólar")]
        [DisplayFormat(DataFormatString = "{0:C4}",
        ApplyFormatInEditMode = true)]
        [DecimalPrecision(18, 4)]
        public decimal precioDelDolar { get; set; }

        [Display(Name = "Divisa")]
        public string divisa { get; set; }

        public static class TipoMovimientoCapital
        {
            public static readonly string ABONO = "ABONO";
            public static readonly string PRESTAMO = "ANTICIPO";
        }

        public static class Divisas
        {
            public static readonly string MXN = "MXN";
            public static readonly string USD = "USD";
        }

        public PrestamoYAbonoCapital() { }

        /// <summary>
        /// Para los movimientos de capital, se ajusta la hora de la fecha de movimiento y el signo del monto
        /// segun el concepto (Prestamo o Abono), esto para preparse para ser mostrado en la lsita de balances
        /// TODO: Checar si es posible hacer estos ajustes sobrecargando los setters de fechaMovimiento, concepto y montoDeMovimiento
        /// </summary>
        public new void ajustarMovimiento()
        {
            //Los prestamos son cantidad negativas, los abonos no indican a que concepto pertenencen.
            if (this.tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO)
                this.montoMovimiento *= -1;
            else if (this.tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO) { 
                this.proveedor = PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO;
                this.fechaPagar = null;
            }

            //Por defecto se establece se introduce el nuevo movimiento en la ultima temporada de cosecha
            if (this.TemporadaDeCosechaID == 0)
                this.TemporadaDeCosechaID = TemporadaDeCosecha.getUltimaTemporada().TemporadaDeCosechaID;

            base.ajustarMovimiento();
        }

        public static SelectList getConceptosSelectList()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var opciones = from item in db.Conceptos.ToList()
                           select new { Value = item.nombreConcepto, Text = item.nombreConcepto };
            return new SelectList(opciones,"Value","Text");
        }

        public static Object[] getTipoMovimientoCapitalArray()
        {
            Object[] opciones = new Object[]{
                new {Value=PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO,
                    Text =PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO},
                new {Value=PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO,
                    Text =PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO},
            };
            return opciones;
        }

        public static Object[] getDivisasArray()
        {
            Object[] opciones = new Object[]{
                new {Value=PrestamoYAbonoCapital.Divisas.MXN,
                    Text =PrestamoYAbonoCapital.Divisas.MXN},
                new {Value=PrestamoYAbonoCapital.Divisas.USD,
                    Text =PrestamoYAbonoCapital.Divisas.USD},
            };
            return opciones;
        }

        public class Proveedores
        {
            [Key]
            public int id { get; set; }

            [Required]
            [Display(Name = "Proveedor")]
            public string nombreProveedor { get; set; }
        }
        public class Conceptos
        {
            [Key]
            public int id { get; set; }

            [Required]
            [Display(Name = "Concepto")]
            public string nombreConcepto { get; set; }
        }
    }
}