namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tabla_Intermedia_CompraProducto : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompraProductoes",
                c => new
                    {
                        compraID = c.Int(nullable: false, identity: true),
                        idProducto = c.Int(),
                        idMovimiento = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.compraID)
                .ForeignKey("dbo.Productoes", t => t.idProducto)
                .ForeignKey("dbo.MovimientoFinancieroes", t => t.idMovimiento, cascadeDelete: true)
                .Index(t => t.idProducto)
                .Index(t => t.idMovimiento);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompraProductoes", "idMovimiento", "dbo.MovimientoFinancieroes");
            DropForeignKey("dbo.CompraProductoes", "idProducto", "dbo.Productoes");
            DropIndex("dbo.CompraProductoes", new[] { "idMovimiento" });
            DropIndex("dbo.CompraProductoes", new[] { "idProducto" });
            DropTable("dbo.CompraProductoes");
        }
    }
}
