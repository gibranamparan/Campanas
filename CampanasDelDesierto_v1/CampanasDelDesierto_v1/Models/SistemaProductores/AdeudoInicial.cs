using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models.SistemaProductores
{
    public class AdeudoInicial:MovimientoFinanciero
    {
        public TipoDeBalance balanceAdeudado { get; set; }
        public new string concepto { get
            {
                return this.nombreDeMovimiento;
            } }
        
        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [DisplayName("Interes Inicial")]
        public decimal interesInicial { get; set; }

        public void ajustarMovimiento()
        {
            this.montoMovimiento *= -1;
            base.ajustarMovimiento();
        }
    }
}