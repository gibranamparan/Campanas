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
        public int idActivos { get; set; }
        public string nombreActivo { get; set; }
        public decimal costo { get; set; }
        public string estado { get; set; }

        public ICollection<PrestamoMaterial> PrestamosMateriales { get; set; }
    }
}