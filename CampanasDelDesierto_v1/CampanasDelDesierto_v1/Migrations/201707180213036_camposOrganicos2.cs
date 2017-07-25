namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class camposOrganicos2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "cantidadProducto4", c => c.Double(defaultValue:0));
            AddColumn("dbo.MovimientoFinancieroes", "pagoProducto4", c => c.Decimal(precision: 18, scale: 2, defaultValue: 0));
            DropColumn("dbo.MovimientoFinancieroes", "pagoProducto7");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "pagoProducto7", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.MovimientoFinancieroes", "pagoProducto4");
            DropColumn("dbo.MovimientoFinancieroes", "cantidadProducto4");
        }
    }
}
