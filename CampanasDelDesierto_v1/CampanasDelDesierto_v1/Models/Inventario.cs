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
        public virtual Sucursal Sucursal { get; set; }                
        public int idSucursal { get; set; }

        //Obtener Activos disponibles del inventario
        public int GetActivosDisponibles(int? id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<int> activo = new List<int>();
            Activo ac = new Activo();
            var inventarioEncontrado = db.Inventarios.Find(id);
            var activos = inventarioEncontrado.Activos.ToList();
            for (int i = 0; i < activos.Count(); i++)
            {
               
                activo.Add(activos.Single().idActivo);
            }
            foreach (var idActivo in activo)
            {
                if (ac.estaPrestado(idActivo))
                {
                    int disponibles = 0;
                    disponibles = disponibles++;
                    return disponibles;
                }
            }
            return 0;
        }
    }
}