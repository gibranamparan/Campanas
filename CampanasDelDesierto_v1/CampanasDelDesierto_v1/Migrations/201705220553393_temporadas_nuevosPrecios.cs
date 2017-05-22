namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class temporadas_nuevosPrecios : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TemporadaDeCosechas", "precioProducto4", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TemporadaDeCosechas", "precioProducto5", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TemporadaDeCosechas", "precioProducto6", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.MovimientoFinancieroes", "pagoProducto1", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.MovimientoFinancieroes", "pagoProducto2", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.MovimientoFinancieroes", "pagoProducto3", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.TemporadaDeCosechas", "precioProductoOtro");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TemporadaDeCosechas", "precioProductoOtro", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.MovimientoFinancieroes", "pagoProducto3", c => c.Double());
            AlterColumn("dbo.MovimientoFinancieroes", "pagoProducto2", c => c.Double());
            AlterColumn("dbo.MovimientoFinancieroes", "pagoProducto1", c => c.Double());
            DropColumn("dbo.TemporadaDeCosechas", "precioProducto6");
            DropColumn("dbo.TemporadaDeCosechas", "precioProducto5");
            DropColumn("dbo.TemporadaDeCosechas", "precioProducto4");
        }
    }
}
