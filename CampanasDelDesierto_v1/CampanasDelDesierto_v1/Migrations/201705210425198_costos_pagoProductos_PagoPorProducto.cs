namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class costos_pagoProductos_PagoPorProducto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "pagoProducto1", c => c.Double());
            AddColumn("dbo.MovimientoFinancieroes", "pagoProducto2", c => c.Double());
            AddColumn("dbo.MovimientoFinancieroes", "pagoProducto3", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "pagoProducto3");
            DropColumn("dbo.MovimientoFinancieroes", "pagoProducto2");
            DropColumn("dbo.MovimientoFinancieroes", "pagoProducto1");
        }
    }
}
