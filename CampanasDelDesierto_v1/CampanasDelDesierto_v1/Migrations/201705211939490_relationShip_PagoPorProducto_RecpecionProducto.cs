namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationShip_PagoPorProducto_RecpecionProducto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RecepcionDeProductoes", "movimientoID", c => c.Int());
            CreateIndex("dbo.RecepcionDeProductoes", "movimientoID");
            AddForeignKey("dbo.RecepcionDeProductoes", "movimientoID", "dbo.MovimientoFinancieroes", "idMovimiento");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecepcionDeProductoes", "movimientoID", "dbo.MovimientoFinancieroes");
            DropIndex("dbo.RecepcionDeProductoes", new[] { "movimientoID" });
            DropColumn("dbo.RecepcionDeProductoes", "movimientoID");
        }
    }
}
