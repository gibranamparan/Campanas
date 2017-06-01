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
  
        //Una sucursal tiene muchos departamentos
        public virtual ICollection<Departamento> Departamentos { get; set; }
    }
}