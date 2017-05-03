using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class Sucursal
    {
        [Key]
        public int idSucursal { get; set; }

        
        [Display(Name = "Nombre Sucursal")]
        public string nombreSucursal { get; set; }

        
        [Display(Name = "Domicilio de Sucursal")]
        public string domicilioSucursal { get; set; }
        

        //Una sucursal tiene muchos inventarios
        public virtual ICollection<Inventario> Inventarios { get; set; }

        //Una sucursal tiene muchos departamentos
        public virtual ICollection<Departamento> Departamentos { get; set; }

        //Cuantos activos hay en el inventario de una sucursal en especifico
        public int GetActivosEnAlmacen(int? id)
        {
            if (id==null)
            {
                return 0;
            }
            else
            { 
            ApplicationDbContext db = new ApplicationDbContext();
            Inventario inventario = db.Inventarios.Find(id);
            var candidadEnAlmacen = inventario.Activos.Where(activo => activo.PrestamosActivos.Last().fechaDevolucion != null);
            return candidadEnAlmacen.Count();
            }
        }
    }
}