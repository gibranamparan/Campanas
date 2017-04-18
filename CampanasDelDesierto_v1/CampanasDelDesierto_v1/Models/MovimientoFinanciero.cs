﻿using System;
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
        [Display(Name = "Monto")]
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

        [Display(Name = "Tipo")]
        public string nombreDeMovimiento
        {
            get
            {
                TypeOfMovements tom = this.getTypeOfMovement();
                if (tom == TypeOfMovements.CAPITAL)
                    return "CAPITAL";
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
                    return ((VentaACredito)this).Producto.nombreProducto;
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
    }
}