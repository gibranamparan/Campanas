using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class Inventario
    {
        [Key]
        public int inventarioID { get; set; }
        
        [Display(Name = "Nombre del inventario")]
        public string nombreInventario { get; set; }
        
        [Display(Name = "Modelo")]
        public string modeloInventario { get; set; }

        [Display(Name = "Costo")]
        public decimal costo { get; set; }

        [Display(Name = "Cantidad")]
        public int cantidad { get; set; }

        //Un inventario tiene muchos activos
        public virtual ICollection<Activo> Activos { get; set; }

        //Un inventario tiene una sucursal              
        public int? departamentoID { get; set; }
        public virtual Departamento Departamento { get; set; }

        /// <summary>
        /// Cuenta la cantidad de activos disponibles en este inventario
        /// </summary>
        public int cantidadActivosDisponibles()
            {
                var noPrestados = this.Activos.ToList().Where(act => !act.prestado());
                return noPrestados.Count();
            }
        
    }
}