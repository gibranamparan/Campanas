﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class PrestamoActivo
    {
        [Key]        
        public int idPrestamoActivo { get; set; }

        [Display(Name = "Fecha de prestamo")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime fechaPrestamoActivo { get; set; }

        [Display(Name = "Fecha de entrega")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime fechaEntregaActivo { get; set; }

        [Display(Name = "Fecha de devolucion")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? fechaDevolucion { get; set; }

        [Display(Name = "Empleado")]
        public int idEmpleado { get; set; }
        public virtual Empleado Empleado { get; set; }

        //Un prestamo contiene muchas adquisiciones de activos
        public virtual ICollection<AdquisicionDeActivo> AdquisicionDeActivos { get; set; }
    }
}