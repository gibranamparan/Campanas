using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class AdminDepartamento:ApplicationUser
    {
        public AdminDepartamento() { }
        public AdminDepartamento(RegisterViewModel model)
        {
            if (!String.IsNullOrEmpty(model.userID)) 
                this.Id = model.userID;

            this.UserName = model.Email;
            this.Email = model.Email;
            this.nombre = model.nombre;
            this.apellidoPaterno = model.apellidoPaterno;
            this.apellidoMaterno = model.apellidoMaterno;
            this.departamentoID = model.departamento;         
        }

        public int? departamentoID { get; set; }
        public virtual Departamento Departamento { get; set; }
    }
}