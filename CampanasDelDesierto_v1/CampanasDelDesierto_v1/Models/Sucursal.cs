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

       
        
    }
}