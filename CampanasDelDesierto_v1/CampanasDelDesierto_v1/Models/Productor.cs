using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CampanasDelDesierto_v1.Models;

namespace CampanasDelDesierto_v1.Models
{
    public class Productor
    {
        [Key]
        public int idProductor { get; set; }

        [Display(Name = "Nombre Productor ")]
        public string nombreProductor { get; set; }

        [Display(Name ="Domicilio")]
        public string domicilio { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de integracion")]
        public DateTime fechaIntegracion { get; set; }

        [Display(Name = "RFC ")]
        public string RFC { get; set; }

        [Display(Name ="Zona")]
        public string zona { get; set; }

        [Display(Name ="Nombre del Cheque")]
        public string nombreCheque { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name ="Adeudo Anterior")]
        public decimal? adeudoAnterior { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name ="Precio de Cosecha")]
        public decimal? precioCosecha { get; set; }

        //Tipos de producto
        public string tipoProducto1 { get { return TiposDeProducto.PRODUCTO1; } }
        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        public decimal costoProducto1 { get; set; }

        public string tipoProducto2 { get { return TiposDeProducto.PRODUCTO2; } }
        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        public decimal costoProducto2 { get; set; }

        public string tipoProductoOtro { get { return TiposDeProducto.OTRO; } }
        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        public decimal costoProductoOtro { get; set; }

        public virtual ICollection<MovimientoFinanciero> MovimientosFinancieros { get; set; }

        public static class TiposDeProducto {
            public static readonly string PRODUCTO1 = "MANZANITA";
            public static readonly string PRODUCTO2 = "MISSION";
            public static readonly string OTRO = "OTRO";
        }

        /// <summary>
        /// Se ajustan todos los balances de los movimientos del productor
        /// desde la fecha inicial indicada.
        /// </summary>
        /// <param name="fechaInicial">Fecha desde la cual se comienzan a corregir los balances.</param>
        internal int ajustarBalances(MovimientoFinanciero ultimoMovimiento, ApplicationDbContext db)
        {
            /*Tomando como referencia el ultimo movimiento anterior al recien modificado, se toman
            todos los registros posteriores a este, en caso de que el recien modificado sea el 1ro,
            se toman por defecto todos los registros existentes*/
            var movimientos = ultimoMovimiento!=null ? this.MovimientosFinancieros
                    .Where(mov => mov.fechaMovimiento > ultimoMovimiento.fechaMovimiento)
                    : this.MovimientosFinancieros;

            //Se crea una lista encadenada ordenada cronologicamente hacia el pasado
            movimientos = movimientos.OrderByDescending(mov => mov.fechaMovimiento).ToList();
            LinkedList<MovimientoFinanciero> movimientosOrdenados = new LinkedList<MovimientoFinanciero>(movimientos);

            //Para el registro recien modificado, se recalcula su balance
            movimientosOrdenados.Last.Value.balance = movimientosOrdenados.Last.Value.montoMovimiento + 
                (ultimoMovimiento==null?0:ultimoMovimiento.balance);

            /*Recorriendo la lista encadenada desde el registro recien modificado hasta el ultimo
            se vam corrigiendo los balances, uno tras otro*/
            int numreg = 0;
            if (movimientos.Count() > 1)
            {
                //apuntador actual
                var nodePointer = movimientosOrdenados.Last;
                while (nodePointer.Previous != null)
                {
                    //nodo anterior
                    var nodo = nodePointer.Previous;

                    //Se calcula nuevo balance
                    nodo.Value.balance = nodePointer.Value.balance + nodo.Value.montoMovimiento;

                    //Se recorre apuntador
                    nodePointer = nodo;
                }
            }

            //Se notifica la edicion de los registros modificados
            foreach (var mov in movimientosOrdenados)
                db.Entry(mov).State = System.Data.Entity.EntityState.Modified;

            //Se guardan cambios
            numreg = db.SaveChanges();

            return numreg;
        }

        public MovimientoFinanciero getUltimoMovimiento()
        {
            MovimientoFinanciero m = this.MovimientosFinancieros
                .OrderByDescending(mov => mov.fechaMovimiento).FirstOrDefault();

            return m;
        }

        internal MovimientoFinanciero getUltimoMovimiento(DateTime fechaMovimiento)
        {
            /*var movs = this.MovimientosFinancieros
                .Where(mov => mov.fechaMovimiento <= fechaMovimiento).ToList()
                .OrderByDescending(mov => mov.fechaMovimiento);

            MovimientoFinanciero m = movs.FirstOrDefault();*/

            var movs = this.MovimientosFinancieros
                .Where(mov => mov.fechaMovimiento <= fechaMovimiento)
                .OrderByDescending(mov => mov.fechaMovimiento)
                .Take(2).ToList();

            MovimientoFinanciero m;
            if (movs.Count() > 0)
                m = null;
            else
                m = movs.ElementAt(movs.Count() - 1);

            return m;
        }
    }
}