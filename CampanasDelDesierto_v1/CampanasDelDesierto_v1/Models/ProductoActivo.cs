using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class ProductoActivo
    {
        [Key]
        public int ProductoActivoID { get; set; }

        [Display(Name = "Numero de Serie")]    
        public string noSerie { get; set; }

        [Display(Name = "Descipcion de Activo")]
        public string descripcionActivo { get; set; }

        [Display(Name = "Observaciones")]
        public string observacionesActivo { get; set; }

        [Display(Name = "Fecha de Prestamo")]
        public DateTime? fechaPrestamo { get; set; }

        [Display(Name = "Fecha de Devolucion")]
        public DateTime? fechaDevolucion { get; set; }
        
        [Display(Name = "Fecha Entregado")]
        public DateTime? fechaEntregado { get; set; }

        //referencia a tabla activo
        [ForeignKey("Activo")]
        public int idActivo { get; set; }
        public virtual Activo Activo { get; set; }

        //Referencia a tabla Adquisicion de activos
        public virtual ICollection<AdquisicionDeActivo> AdquisicionesDeActivos { get; set; }

        //Metodo productoActivo Prestado
        [Display(Name = "Actualmente Prestado")]
        public bool prestado()
        {
            bool prestado = false;
            if (this.fechaPrestamo != null)
            {
                if (this.fechaEntregado != null)
                {
                    return prestado;
                }
                else
                {
                    return prestado = true;
                }
            }
            else
            {
                return prestado = false;
            }
            

        }

    }
}