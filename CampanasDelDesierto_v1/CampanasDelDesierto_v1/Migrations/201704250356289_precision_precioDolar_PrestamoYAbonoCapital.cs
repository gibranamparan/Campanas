namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class precision_precioDolar_PrestamoYAbonoCapital : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MovimientoFinancieroes", "precioDelDolar", c => c.Decimal(precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MovimientoFinancieroes", "precioDelDolar", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
