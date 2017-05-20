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
        [Display(Name = "Monto (USD)")]
        public decimal montoMovimiento { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Balance (USD)")]
        public decimal balance { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime fechaMovimiento { get; set; }

        //Todos los movimientos financieros le corresponden a un solo productor
        public int idProductor { get; set; }
        public virtual Productor Productor { get; set; }

        //Cada movimiento entra dentro de un periodo de cosecha previamente aperturado
        public int TemporadaDeCosechaID { get; set; }
        public virtual TemporadaDeCosecha temporadaDeCosecha { get; set; }

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
                        return "ANTICIPO";
                }
                else if (tom == TypeOfMovements.PAGO_POR_PRODUCTO)
                    return "COSECHA";
                else if (tom == TypeOfMovements.VENTA_A_CREDITO)
                    return "VENTA A CREDITO";
                else if (tom == TypeOfMovements.CHEQUE)
                    return "CHEQUE";
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
                    return "INGRESO DE PRODUCTO";
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
            VENTA_A_CREDITO,
            CHEQUE
        };

        public TypeOfMovements getTypeOfMovement()
        {
            if (this is PagoPorProducto)
                return TypeOfMovements.PAGO_POR_PRODUCTO;
            else if (this is PrestamoYAbonoCapital)
                return TypeOfMovements.CAPITAL;
            else if (this is VentaACredito)
                return TypeOfMovements.VENTA_A_CREDITO;
            else if (this is EmisionDeCheque)
                return TypeOfMovements.CHEQUE;
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
            else if (tom == TypeOfMovements.CHEQUE)
                return "EmisionDeCheques";
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
        internal void introducirMovimientoEnPeriodo(int? periodoID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            introducirMovimientoEnPeriodo(periodoID, db);
        }

        public void introducirMovimientoEnPeriodo(int? periodoID, ApplicationDbContext db)
        {
            TemporadaDeCosecha tem = TemporadaDeCosecha.findTemporada(periodoID);
            introducirMovimientoEnPeriodo(tem);
        }

        public void introducirMovimientoEnPeriodo(TemporadaDeCosecha tem)
        {
            this.temporadaDeCosecha = tem;
            this.TemporadaDeCosechaID = tem.TemporadaDeCosechaID;
            int anioCosecha = tem.fechaFin.Year;
            //Si la fecha determinada no entra dentro del periodo de cosecha
            this.fechaMovimiento = new DateTime(anioCosecha, DateTime.Now.Month, DateTime.Now.Day);
            //Si es mayor al rango de periodo de cosecha
            if (this.fechaMovimiento > tem.fechaFin)
                this.fechaMovimiento = tem.fechaFin; //Se le resta un año
            else if (this.fechaMovimiento < tem.fechaInicio)
                this.fechaMovimiento = tem.fechaInicio; //Se le suma un año
        }

        private int DIA_INICIO_PERIODO = 30;
        private int DIA_FIN_PERIODO = 30;
        private int MES_PERIODO = 8;

        public int anioCosecha
        {
            get
          {
               int anioCosecha = this.fechaMovimiento.Year;
               if (this.fechaMovimiento > new DateTime(this.fechaMovimiento.Year,
                      this.MES_PERIODO, this.DIA_INICIO_PERIODO))
                  anioCosecha++;
               return anioCosecha;
            }
        }


    }
}