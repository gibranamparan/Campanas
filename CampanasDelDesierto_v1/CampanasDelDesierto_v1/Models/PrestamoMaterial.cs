using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class PrestamoMaterial:MovimientoFinanciero
    {
        public int cantidadMaterial { get; set; }


        public virtual Activo Activo { get; set; }
        public virtual int idActivos { get; set; }


    }
}