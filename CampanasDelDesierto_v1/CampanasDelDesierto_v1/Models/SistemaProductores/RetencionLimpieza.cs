using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class RetencionLimpieza : MovimientoFinanciero
    {
        public new void ajustarMovimiento()
        {
            this.montoMovimiento *= -1;
            base.ajustarMovimiento();
        }

        public RetencionLimpieza() { }
        public RetencionLimpieza(EmisionDeCheque emisionDeCheque, decimal monto)
        {
            this.montoMovimiento = monto;
            this.fechaMovimiento = emisionDeCheque.fechaMovimiento;
            this.TemporadaDeCosechaID = emisionDeCheque.TemporadaDeCosechaID;
            this.idProductor = emisionDeCheque.idProductor;
        }
    }
}