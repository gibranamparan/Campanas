using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class PrestamoYAbonoCapital:MovimientoFinanciero
    {
        //public int MesesAPagar { get; set; }
        //[DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}",
        // ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        //public DateTime fechaDePrestamo { get; set; }
        // public int rate { get; set; }
        [Display(Name = "Cheque/Folio")]
        public string cheque { get; set; }
        [Display(Name = "Concepto")]
        public string concepto { get; set; }

        //public double cargo { get; set; }
        [Display(Name = "Pagaré")]
        public string pagare { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha a pagar")]
        public DateTime fechaPagar { get; set; }
        [Display(Name = "Proveedor")]
        public string proveedor { get; set; }

        //public int intereses { get; set; }
        [Display(Name = "Nota")]
        public string nota { get; set; }

        public static class TipoMovimientoCapital
        {
            public static readonly string ABONO = "ABONO";
            public static readonly string PRESTAMO = "PRESTAMO";
        }

        /// <summary>
        /// Para los movimientos de capital, se ajusta la hora de la fecha de movimiento y el signo del monto
        /// segun el concepto (Prestamo o Abono), esto para preparse para ser mostrado en la lsita de balances
        /// TODO: Checar si es posible hacer estos ajustes sobrecargando los setters de fechaMovimiento, concepto y montoDeMovimiento
        /// </summary>
        public new void ajustarMovimiento()
        {
            //Los prestamos son cantidad negativas, los abonos no indican a que concepto pertenencen.
            if (this.concepto == PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO)
                this.montoMovimiento *= -1;
            else if (this.concepto == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO)
                this.proveedor = PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO;

            base.ajustarMovimiento();
        }

        public static Object[] getTipoMovimientoCapitalArray()
        {
            Object[] opciones = new Object[]{
                new {Value=PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO, Text="ABONO"},
                new {Value=PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO, Text="PRESTAMO"},
            };
            return opciones;
        }
    }
}