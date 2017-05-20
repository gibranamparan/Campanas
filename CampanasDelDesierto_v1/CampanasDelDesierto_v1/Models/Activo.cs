using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class Activo
    {
        [Key]
        public int idActivo { get; set; }

        [Display(Name = "Nombre de Activo")]
        public string nombreActivo { get; set; }

        [Display(Name = "Estado de Activo")]
        public string estadoActivo { get; set; }

        //Un activo tiene una collecion de prestamos
        public virtual ICollection<PrestamoActivo> PrestamosActivos { get; set; }

        //Cada activo pertenece a un inventario
        public int inventarioID { get; set; }
        public virtual Inventario inventario { get; set; }

        [Display(Name = "Actualmente Prestado")]
        public bool prestado() {


                bool prestado = false;
        List<PrestamoActivo> prestamo = this.PrestamosActivos.ToList();

                if (prestamo.Count() > 0)
                    prestado = prestamo.Last().fechaDevolucion == null;

                return prestado;
          
        }
    }
}