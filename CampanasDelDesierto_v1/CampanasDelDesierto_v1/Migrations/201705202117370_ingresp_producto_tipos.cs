namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ingresp_producto_tipos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "cantidadProducto1", c => c.Double());
            AddColumn("dbo.MovimientoFinancieroes", "cantidadProducto2", c => c.Double());
            AddColumn("dbo.MovimientoFinancieroes", "cantidadProducto3", c => c.Double());
            DropColumn("dbo.MovimientoFinancieroes", "cantidadProducto");
            DropColumn("dbo.MovimientoFinancieroes", "tipoProducto");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "tipoProducto", c => c.String());
            AddColumn("dbo.MovimientoFinancieroes", "cantidadProducto", c => c.Double());
            DropColumn("dbo.MovimientoFinancieroes", "cantidadProducto3");
            DropColumn("dbo.MovimientoFinancieroes", "cantidadProducto2");
            DropColumn("dbo.MovimientoFinancieroes", "cantidadProducto1");
        }
    }
}
