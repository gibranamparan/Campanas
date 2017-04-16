using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    /// <summary>
    /// Representa un registro de compra de cosecha vendida por un productor
    /// </summary>
    public class PagoPorProducto : MovimientoFinanciero
    {
        [Display(Name = "Cantidad de cosecha")]
        public double cantidadProducto { get; set; }

        [Display(Name = "Numero de la semana")]
        public int numeroSemana { get; set; }

        [Display(Name = "Cheque ")]
        public string cheque { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Abono de anticipo ")]
        public decimal abonoAnticipo { get; set; }

        [Display(Name = "Tipo de producto ")]
        public string tipoProducto { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Pago de garantia de la limpieza")]
        public decimal garantiaLimpieza { get; set; }


        /// <summary>
        /// Para los movimientos de capital, se ajusta la hora de la fecha de movimiento y el signo del monto
        /// segun el concepto (Prestamo o Abono), esto para preparse para ser mostrado en la lsita de balances
        /// TODO: Checar si es posible hacer estos ajustes sobrecargando los setters de fechaMovimiento, concepto y montoDeMovimiento
        /// </summary>
        public void ajustarMovimiento()
        {
            //Se registra el nuevo movimiento
            //Se agrega la hora de registro a la fecha del movimiento solo para diferencia movimientos hecho el mismo dia
            this.fechaMovimiento = this.fechaMovimiento
                .AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute).AddSeconds(DateTime.Now.Second);
        }
    }
}