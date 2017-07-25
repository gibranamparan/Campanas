namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class camposOrganicos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "pagoProducto7", c => c.Decimal(precision: 18, scale: 2, defaultValue: 0));
            AddColumn("dbo.RecepcionDeProductoes", "cantidadTonsProd4", c => c.Double(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RecepcionDeProductoes", "cantidadTonsProd4");
            DropColumn("dbo.MovimientoFinancieroes", "pagoProducto7");
        }
    }
}
