using CampanasDelDesierto_v1.HerramientasGenerales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualBasic;
using System.ComponentModel;

namespace CampanasDelDesierto_v1.Models
{
    public class PrestamoYAbonoCapital : MovimientoFinanciero
    {
        [Display(Name = "Cheque/Folio")]
        public string cheque { get; set; }

        /// <summary>
        /// Indica el tipo de movimiento de Abono O Capital, este puede ser ABONO, ANTICIPO, ABONO A ARBOLES
        /// </summary>
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

        public string conceptoCapital { get; set; }
        
        [Display(Name = "Concepto")]
        public new string concepto {
            get {
                string res = this.conceptoCapital;
                /* if (this.tipoDeMovimientoDeCapital != PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO)
                     res = this.tipoDeMovimientoDeCapital;*/
                return res;
            }
            set { this.conceptoCapital = value; } }

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

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Monto (MXN)")]
        public decimal montoPesos { get; set; }

        [Display(Name = "Divisa")]
        public string divisa { get; set; }

        public int? abonoEnliquidacionID { get; set; }

        /// <summary>
        /// Conjunto de intereses generados para este prestamo
        /// </summary>
        public ICollection<CargoDeInteres> intereses { get; set; }

        /// <summary>
        /// Si es abono, prestamos sobre los cuales se distribuye el abono
        /// </summary>
        [InverseProperty("abono")]
        public virtual ICollection<Prestamo_Abono> prestamosAbonados { get; set; }

        public static class TipoMovimientoCapital
        {
            public static readonly string ABONO = "ABONO";
            public static readonly string PRESTAMO = "ANTICIPO";
            public static readonly string ABONO_ARBOLES = "ABONO A ARBOLES";
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
        public new void ajustarMovimiento(DateTime? originalDate = null)
        {
            //Los prestamos son cantidad negativas, los abonos no indican a que concepto pertenencen.
            if (this.tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO)
                this.montoMovimiento *= -1;
            else if (this.tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO
                || this.tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO_ARBOLES) { 
                this.proveedor = this.tipoDeMovimientoDeCapital; //confirmar
                this.conceptoCapital = this.proveedor;
                //this.pagare = string.Empty;
                this.fechaPagar = null;
            }

            //Por defecto se establece se introduce el nuevo movimiento en la ultima temporada de cosecha
            if (this.TemporadaDeCosechaID == 0)
                this.TemporadaDeCosechaID = TemporadaDeCosecha.getUltimaTemporada().TemporadaDeCosechaID;

            base.ajustarMovimiento(originalDate);
        }

        public static PrestamoYAbonoCapital nuevaRentecionAbono(LiquidacionSemanal ls, decimal monto, string tipoCapital)
        {
            PrestamoYAbonoCapital abono = new PrestamoYAbonoCapital();
            //abono.liquidacionDondeAbonaID = ls.idMovimiento;
            abono.fechaMovimiento = ls.fechaMovimiento.AddSeconds(-5);
            abono.precioDelDolar = ls.precioDelDolarEnLiquidacion;
            abono.concepto = tipoCapital+" EN LIQUIDACION (CH:" + ls.cheque + ")";
            abono.montoMovimiento = monto;
            abono.TemporadaDeCosechaID = ls.TemporadaDeCosechaID;
            abono.idProductor = ls.idProductor;
            abono.abonoEnliquidacionID = ls.idMovimiento;
            abono.tipoDeMovimientoDeCapital = tipoCapital;

            return abono;
        }

        //TODO: Terminar de desarrollar metodo para generar movimientos de interes
        internal void incrementarInteres(ApplicationDbContext db)
        {
            double totalPays = 12;
            double interest = .1 / totalPays;

            double toPay = Financial.Pmt(interest, totalPays, (double)this.montoMovimiento / totalPays);
            //CargoDeInteres  
        }

        public static SelectList getConceptosSelectList()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var opciones = from item in db.Conceptos.ToList()
                           select new { Value = item.nombreConcepto, Text = item.nombreConcepto };
            opciones = opciones.OrderBy(op => op.Value);
            return new SelectList(opciones,"Value","Text");
        }

        /// <summary>
        /// Arroja una lista de opciones correspondientes a los tipos de movimiento de capital posibles que son PRESTAMO, 
        /// ABONO  y ABONO A ARBOLES.
        /// </summary>
        /// <param name="soloAbonos">Por defecto es falso, en caso de ser verdadero, arroja una lista de opciones donde solamente se ven
        /// los nombres de los tipos de abonos, omitiendo los prestamos o anticipos.</param>
        /// <returns></returns>
        public static List<object> getTipoMovimientoCapitalArray(bool soloAbonos = false)
        {
            List<object> opciones = new List<object>();

            if (!soloAbonos)
                opciones.Add(new
                {
                    Value = PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO,
                    Text = PrestamoYAbonoCapital.TipoMovimientoCapital.PRESTAMO
                });

            opciones.Add(new
            {
                Value = PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO,
                Text = PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO
            });

            opciones.Add(new
            {
                Value = PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO_ARBOLES,
                Text = PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO_ARBOLES
            });

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

        public class CargoDeInteres
        {
            [Key]
            public int cargoDeInteresID { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [Display(Name = "Monto (USD)")]
            public decimal monto { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
            ApplyFormatInEditMode = true)]
            [DataType(DataType.Date)]
            public DateTime fecha { get; set; }

            [ForeignKey("anticipo")]
            public int anticipoID { get; set; }
            public virtual PrestamoYAbonoCapital anticipo { get; set; }
        }

        public int liberarAbono(ApplicationDbContext db)
        {
            var prestamoAbonos = db.Prestamo_Abono.Where(mov => mov.abonoID == this.idMovimiento);
            db.Prestamo_Abono.RemoveRange(prestamoAbonos);
            return db.SaveChanges();
        }

        public class Prestamo_Abono
        {
            public Prestamo_Abono() { }
            public Prestamo_Abono(MovimientoFinanciero prestamo, PrestamoYAbonoCapital abono)
            {
                this.prestamoID = prestamo.idMovimiento;
                this.abonoID = abono.idMovimiento;
                this.prestamo = prestamo;
                this.abono = abono;
            }

            public int id { get; set; }

            [ForeignKey("prestamo")]
            public int? prestamoID { get; set; }
            public virtual MovimientoFinanciero prestamo { get; set; }

            [ForeignKey("abono")]
            public int? abonoID { get; set; }
            public virtual PrestamoYAbonoCapital abono { get; set; }

            [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
            [Display(Name = "Monto (USD)")]
            public decimal monto { get; set; }

            public bool pagoAInteres { get; set; }
        }

    }
}