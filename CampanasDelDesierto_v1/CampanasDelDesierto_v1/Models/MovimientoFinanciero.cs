using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class MovimientoFinanciero
    {
        [Key]
        public int idMovimiento { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Monto (dólares)")]
        public decimal montoMovimiento { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Balance ")]
        public decimal balance { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime fechaMovimiento { get; set; }

        //Todos los movimientos financieros le corresponden a un solo productor
        public int idProductor { get; set; }
        public virtual Productor Productor { get; set; }

        private int DIA_INICIO_PERIODO = 30;
        private int DIA_FIN_PERIODO = 30;
        private int MES_PERIODO = 8;

        public int anioCosecha { get {
                int anioCosecha = this.fechaMovimiento.Year;
                if (this.fechaMovimiento > new DateTime(this.fechaMovimiento.Year, 
                        this.MES_PERIODO, this.DIA_INICIO_PERIODO))
                    anioCosecha++;
                return anioCosecha;
            }
        }

        public DateTime inicioCosecha
        {
            get {return new DateTime(anioCosecha - 1, MES_PERIODO, DIA_INICIO_PERIODO); }
        }

        public DateTime finCosecha
        {
            get {return new DateTime(anioCosecha, MES_PERIODO, DIA_INICIO_PERIODO); }
        }

        [Display(Name = "Tipo")]
        public string nombreDeMovimiento
        {
            get
            {
                TypeOfMovements tom = this.getTypeOfMovement();
                if (tom == TypeOfMovements.CAPITAL) {
                    if (this.conceptoProveedor == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO)
                        return PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO;
                    else if (this.conceptoProveedor == PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO)
                        return PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO;
                    else
                        return "CAPITAL";
                }
                else if (tom == TypeOfMovements.PAGO_POR_PRODUCTO)
                    return "COSECHA";
                else if (tom == TypeOfMovements.VENTA_A_CREDITO)
                    return "VENTA A CREDITO";
                else
                    return "";
            }
        }

        [Display(Name = "Tipo")]
        public string conceptoProveedor
        {
            get
            {
                TypeOfMovements tom = this.getTypeOfMovement();
                if (tom == TypeOfMovements.CAPITAL)
                    return ((PrestamoYAbonoCapital)this).proveedor;
                else if (tom == TypeOfMovements.PAGO_POR_PRODUCTO)
                    return ((PagoPorProducto)this).tipoProducto;
                else if (tom == TypeOfMovements.VENTA_A_CREDITO)
                    return ((VentaACredito)this).cantidadMaterial
                        +" "+((VentaACredito)this).Producto.nombreProducto;
                else
                    return "";
            }
        }

        public enum TypeOfMovements
        {
            NONE,
            PAGO_POR_PRODUCTO,
            CAPITAL,
            VENTA_A_CREDITO
        };

        public TypeOfMovements getTypeOfMovement()
        {
            if (this is PagoPorProducto)
                return TypeOfMovements.PAGO_POR_PRODUCTO;
            else if (this is PrestamoYAbonoCapital)
                return TypeOfMovements.CAPITAL;
            else if (this is VentaACredito)
                return TypeOfMovements.VENTA_A_CREDITO;
            else
                return TypeOfMovements.NONE;
        }

        public string getNombreControlador()
        {
            TypeOfMovements tom = this.getTypeOfMovement();
            if (tom == TypeOfMovements.CAPITAL)
                return "PrestamoYAbonoCapitals";
            else if (tom == TypeOfMovements.PAGO_POR_PRODUCTO)
                return "PagosPorProductos";
            else if (tom == TypeOfMovements.VENTA_A_CREDITO)
                return "VentaACreditos";
            else
                return "";
        }

        public void ajustarMovimiento()
        {
            //Se agrega la hora de registro a la fecha del movimiento solo para
            //diferencia movimientos hecho el mismo dia
            this.fechaMovimiento = this.fechaMovimiento
                .AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute)
                .AddSeconds(DateTime.Now.Second);
        }

        /// <summary>
        /// Introduce la fecha del movimiento dentro del año de periodo de cosecha indicado, tomando por defecto
        /// el día y mes de la fecha de registro.
        /// </summary>
        /// <param name="anioCosecha">Año que representa un periodo de cosecha</param>
        internal void introducirMovimientoEnPeriodo(int? anioCosecha)
        {
            //Si se determina el periodo de cosecha, se establece por defecto el año de la fecha del movimiento
            if (anioCosecha == null || anioCosecha == 0)
                anioCosecha = DateTime.Now.Year;

            //Si la fecha determinada no entra dentro del periodo de cosecha
            this.fechaMovimiento = new DateTime(anioCosecha.Value,DateTime.Now.Month,DateTime.Now.Day);
            //Si es mayor al rango de periodo de cosecha
            if (this.fechaMovimiento > new DateTime(anioCosecha.Value, this.MES_PERIODO, this.DIA_FIN_PERIODO))
                this.fechaMovimiento = this.fechaMovimiento.AddYears(-1); //Se le resta un año
            else if ((this.fechaMovimiento < new DateTime(anioCosecha.Value-1, this.MES_PERIODO, this.DIA_FIN_PERIODO)))
                this.fechaMovimiento = this.fechaMovimiento.AddYears(1); //Se le suma un año
        }
    }
}