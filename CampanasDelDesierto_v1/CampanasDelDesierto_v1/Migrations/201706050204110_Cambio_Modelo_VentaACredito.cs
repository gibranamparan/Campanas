namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cambio_Modelo_VentaACredito : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MovimientoFinancieroes", "idProducto", "dbo.Productoes");
            DropIndex("dbo.MovimientoFinancieroes", new[] { "idProducto" });
            DropColumn("dbo.MovimientoFinancieroes", "idProducto");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "idProducto", c => c.Int());
            CreateIndex("dbo.MovimientoFinancieroes", "idProducto");
            AddForeignKey("dbo.MovimientoFinancieroes", "idProducto", "dbo.Productoes", "idProducto", cascadeDelete: true);
        }
    }
}
