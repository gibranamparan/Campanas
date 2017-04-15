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

        public virtual ICollection<MovimientoFinanciero> MovimientosFinancieros { get; set; }

        /// <summary>
        /// Se ajustan todos los balances de los movimientos del productor
        /// desde la fecha inicial indicada.
        /// </summary>
        /// <param name="fechaInicial">Fecha desde la cual se comienzan a corregir los balances.</param>
        internal int ajustarBalances(MovimientoFinanciero ultimoMovimiento, ApplicationDbContext db)
        {
            var movimientos = ultimoMovimiento!=null ? this.MovimientosFinancieros
                    .Where(mov => mov.fechaMovimiento > ultimoMovimiento.fechaMovimiento)
                    : this.MovimientosFinancieros;

            movimientos = movimientos.OrderByDescending(mov => mov.fechaMovimiento).ToList();

            LinkedList<MovimientoFinanciero> movimientosOrdenados = new LinkedList<MovimientoFinanciero>(movimientos);
            movimientosOrdenados.Last.Value.balance = movimientosOrdenados.Last.Value.montoMovimiento + 
                (ultimoMovimiento==null?0:ultimoMovimiento.balance);

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

            foreach (var mov in movimientosOrdenados)
                db.Entry(mov).State = System.Data.Entity.EntityState.Modified;

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
            var movs = this.MovimientosFinancieros
                .Where(mov => mov.fechaMovimiento <= fechaMovimiento).ToList()
                .OrderByDescending(mov => mov.fechaMovimiento);

            MovimientoFinanciero m = movs.FirstOrDefault();

            return m;
        }
    }
}