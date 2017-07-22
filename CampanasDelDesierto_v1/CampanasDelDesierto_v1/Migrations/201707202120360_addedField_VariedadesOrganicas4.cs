namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedField_VariedadesOrganicas4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "cantidadProducto5", c => c.Double());
            AddColumn("dbo.MovimientoFinancieroes", "cantidadProducto6", c => c.Double());
            AddColumn("dbo.MovimientoFinancieroes", "pagoProducto5", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.MovimientoFinancieroes", "pagoProducto6", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "pagoProducto6");
            DropColumn("dbo.MovimientoFinancieroes", "pagoProducto5");
            DropColumn("dbo.MovimientoFinancieroes", "cantidadProducto6");
            DropColumn("dbo.MovimientoFinancieroes", "cantidadProducto5");
        }
    }
}
