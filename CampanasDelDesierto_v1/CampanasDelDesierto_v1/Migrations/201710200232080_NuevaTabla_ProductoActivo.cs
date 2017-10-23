namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NuevaTabla_ProductoActivo : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdquisicionDeActivoes", "idActivo", "dbo.Activoes");
            DropForeignKey("dbo.PrestamoActivoes", "Activo_idActivo", "dbo.Activoes");
            DropIndex("dbo.PrestamoActivoes", new[] { "Activo_idActivo" });
            DropIndex("dbo.AdquisicionDeActivoes", new[] { "idActivo" });
            CreateTable(
                "dbo.ProductoActivoes",
                c => new
                    {
                        ProductoActivoID = c.Int(nullable: false, identity: true),
                        noSerie = c.String(),
                        descripcionActivo = c.String(),
                        observacionesActivo = c.String(),
                        fechaPrestamo = c.DateTime(),
                        fechaDevolucion = c.DateTime(),
                        fechaEntregado = c.DateTime(),
                        idActivo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductoActivoID)
                .ForeignKey("dbo.Activoes", t => t.idActivo, cascadeDelete: true)
                .Index(t => t.idActivo);
            
            AddColumn("dbo.AdquisicionDeActivoes", "ProductoActivoID", c => c.Int());
            CreateIndex("dbo.AdquisicionDeActivoes", "ProductoActivoID");
            AddForeignKey("dbo.AdquisicionDeActivoes", "ProductoActivoID", "dbo.ProductoActivoes", "ProductoActivoID");
            DropColumn("dbo.PrestamoActivoes", "Activo_idActivo");
            DropColumn("dbo.AdquisicionDeActivoes", "idActivo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AdquisicionDeActivoes", "idActivo", c => c.Int());
            AddColumn("dbo.PrestamoActivoes", "Activo_idActivo", c => c.Int());
            DropForeignKey("dbo.AdquisicionDeActivoes", "ProductoActivoID", "dbo.ProductoActivoes");
            DropForeignKey("dbo.ProductoActivoes", "idActivo", "dbo.Activoes");
            DropIndex("dbo.ProductoActivoes", new[] { "idActivo" });
            DropIndex("dbo.AdquisicionDeActivoes", new[] { "ProductoActivoID" });
            DropColumn("dbo.AdquisicionDeActivoes", "ProductoActivoID");
            DropTable("dbo.ProductoActivoes");
            CreateIndex("dbo.AdquisicionDeActivoes", "idActivo");
            CreateIndex("dbo.PrestamoActivoes", "Activo_idActivo");
            AddForeignKey("dbo.PrestamoActivoes", "Activo_idActivo", "dbo.Activoes", "idActivo");
            AddForeignKey("dbo.AdquisicionDeActivoes", "idActivo", "dbo.Activoes", "idActivo");
        }
    }
}
