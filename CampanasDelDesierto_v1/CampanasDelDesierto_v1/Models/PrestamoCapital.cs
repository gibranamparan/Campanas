using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class PrestamoCapital:MovimientoFinanciero
    {
        
        public int MesesAPagar { get; set; }



    }
}