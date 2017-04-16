namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambioDeDatosDinero_Decimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MovimientoFinancieroes", "montoMovimiento", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.MovimientoFinancieroes", "balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.MovimientoFinancieroes", "abonoAnticipo", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.MovimientoFinancieroes", "garantiaLimpieza", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MovimientoFinancieroes", "garantiaLimpieza", c => c.Double());
            AlterColumn("dbo.MovimientoFinancieroes", "abonoAnticipo", c => c.Double());
            AlterColumn("dbo.MovimientoFinancieroes", "balance", c => c.Double(nullable: false));
            AlterColumn("dbo.MovimientoFinancieroes", "montoMovimiento", c => c.Double(nullable: false));
        }
    }
}
