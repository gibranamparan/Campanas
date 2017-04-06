using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class Proveedores
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Display(Name = "Nombre Proveedor")]
        public string nombreProveedor { get; set; }
    }
}