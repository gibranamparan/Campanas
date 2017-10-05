namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modificacion_Modelos_SistemaActivos : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Activoes", "Inventario_inventarioID", "dbo.Inventarios");
            DropForeignKey("dbo.Inventarios", "departamentoID", "dbo.Departamentoes");
            DropIndex("dbo.Activoes", new[] { "Inventario_inventarioID" });
            DropIndex("dbo.Inventarios", new[] { "departamentoID" });
            CreateTable(
                "dbo.CategoriaDeActivoes",
                c => new
                    {
                        CategoriaActivoID = c.Int(nullable: false, identity: true),
                        nombreCategoria = c.String(),
                        departamentoID = c.Int(),
                    })
                .PrimaryKey(t => t.CategoriaActivoID)
                .ForeignKey("dbo.Departamentoes", t => t.departamentoID)
                .Index(t => t.departamentoID);
            
            AddColumn("dbo.Activoes", "CategoriaActivoID", c => c.Int());
            CreateIndex("dbo.Activoes", "CategoriaActivoID");
            AddForeignKey("dbo.Activoes", "CategoriaActivoID", "dbo.CategoriaDeActivoes", "CategoriaActivoID");
            DropColumn("dbo.Activoes", "Inventario_inventarioID");
            DropTable("dbo.Inventarios");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Inventarios",
                c => new
                    {
                        inventarioID = c.Int(nullable: false, identity: true),
                        nombreInventario = c.String(),
                        modeloInventario = c.String(),
                        costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        cantidad = c.Int(nullable: false),
                        departamentoID = c.Int(),
                    })
                .PrimaryKey(t => t.inventarioID);
            
            AddColumn("dbo.Activoes", "Inventario_inventarioID", c => c.Int());
            DropForeignKey("dbo.CategoriaDeActivoes", "departamentoID", "dbo.Departamentoes");
            DropForeignKey("dbo.Activoes", "CategoriaActivoID", "dbo.CategoriaDeActivoes");
            DropIndex("dbo.CategoriaDeActivoes", new[] { "departamentoID" });
            DropIndex("dbo.Activoes", new[] { "CategoriaActivoID" });
            DropColumn("dbo.Activoes", "CategoriaActivoID");
            DropTable("dbo.CategoriaDeActivoes");
            CreateIndex("dbo.Inventarios", "departamentoID");
            CreateIndex("dbo.Activoes", "Inventario_inventarioID");
            AddForeignKey("dbo.Inventarios", "departamentoID", "dbo.Departamentoes", "departamentoID");
            AddForeignKey("dbo.Activoes", "Inventario_inventarioID", "dbo.Inventarios", "inventarioID");
        }
    }
}
