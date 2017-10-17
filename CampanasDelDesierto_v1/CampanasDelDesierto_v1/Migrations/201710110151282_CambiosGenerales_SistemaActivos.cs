namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambiosGenerales_SistemaActivos : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Activoes", "inventarioID", "dbo.Inventarios");
            DropForeignKey("dbo.Inventarios", "departamentoID", "dbo.Departamentoes");
            DropForeignKey("dbo.PrestamoActivoes", "idActivo", "dbo.Activoes");
            DropIndex("dbo.Activoes", new[] { "inventarioID" });
            DropIndex("dbo.Inventarios", new[] { "departamentoID" });
            DropIndex("dbo.PrestamoActivoes", new[] { "idActivo" });
            RenameColumn(table: "dbo.PrestamoActivoes", name: "idActivo", newName: "Activo_idActivo");
            CreateTable(
                "dbo.CategoriaDeActivoes",
                c => new
                    {
                        CategoriaActivoID = c.Int(nullable: false, identity: true),
                        nombreCategoria = c.String(),
                    })
                .PrimaryKey(t => t.CategoriaActivoID);
            
            CreateTable(
                "dbo.AdquisicionDeActivoes",
                c => new
                    {
                        AdquirirActivosID = c.Int(nullable: false, identity: true),
                        cantidadActivo = c.Int(nullable: false),
                        idActivo = c.Int(),
                        idPrestamoActivo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AdquirirActivosID)
                .ForeignKey("dbo.Activoes", t => t.idActivo)
                .ForeignKey("dbo.PrestamoActivoes", t => t.idPrestamoActivo, cascadeDelete: true)
                .Index(t => t.idActivo)
                .Index(t => t.idPrestamoActivo);
            
            AddColumn("dbo.Activoes", "partidaNumActivo", c => c.String());
            AddColumn("dbo.Activoes", "codigoActivo", c => c.String());
            AddColumn("dbo.Activoes", "descripcionActivo", c => c.String());
            AddColumn("dbo.Activoes", "unidadesActivo", c => c.String());
            AddColumn("dbo.Activoes", "mmcUnidades", c => c.String());
            AddColumn("dbo.Activoes", "observacionesActivo", c => c.String());
            AddColumn("dbo.Activoes", "contabilidadActivo", c => c.String());
            AddColumn("dbo.Activoes", "isPrestado", c => c.Boolean(nullable: false));
            AddColumn("dbo.Activoes", "CategoriaActivoID", c => c.Int());
            AddColumn("dbo.Activoes", "departamentoID", c => c.Int());
            AlterColumn("dbo.PrestamoActivoes", "Activo_idActivo", c => c.Int());
            CreateIndex("dbo.Activoes", "CategoriaActivoID");
            CreateIndex("dbo.Activoes", "departamentoID");
            CreateIndex("dbo.PrestamoActivoes", "Activo_idActivo");
            AddForeignKey("dbo.Activoes", "CategoriaActivoID", "dbo.CategoriaDeActivoes", "CategoriaActivoID");
            AddForeignKey("dbo.Activoes", "departamentoID", "dbo.Departamentoes", "departamentoID");
            AddForeignKey("dbo.PrestamoActivoes", "Activo_idActivo", "dbo.Activoes", "idActivo");
            DropColumn("dbo.Activoes", "nombreActivo");
            DropColumn("dbo.Activoes", "estadoActivo");
            DropColumn("dbo.Activoes", "inventarioID");
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
            
            AddColumn("dbo.Activoes", "inventarioID", c => c.Int(nullable: false));
            AddColumn("dbo.Activoes", "estadoActivo", c => c.String());
            AddColumn("dbo.Activoes", "nombreActivo", c => c.String());
            DropForeignKey("dbo.PrestamoActivoes", "Activo_idActivo", "dbo.Activoes");
            DropForeignKey("dbo.AdquisicionDeActivoes", "idPrestamoActivo", "dbo.PrestamoActivoes");
            DropForeignKey("dbo.AdquisicionDeActivoes", "idActivo", "dbo.Activoes");
            DropForeignKey("dbo.Activoes", "departamentoID", "dbo.Departamentoes");
            DropForeignKey("dbo.Activoes", "CategoriaActivoID", "dbo.CategoriaDeActivoes");
            DropIndex("dbo.AdquisicionDeActivoes", new[] { "idPrestamoActivo" });
            DropIndex("dbo.AdquisicionDeActivoes", new[] { "idActivo" });
            DropIndex("dbo.PrestamoActivoes", new[] { "Activo_idActivo" });
            DropIndex("dbo.Activoes", new[] { "departamentoID" });
            DropIndex("dbo.Activoes", new[] { "CategoriaActivoID" });
            AlterColumn("dbo.PrestamoActivoes", "Activo_idActivo", c => c.Int(nullable: false));
            DropColumn("dbo.Activoes", "departamentoID");
            DropColumn("dbo.Activoes", "CategoriaActivoID");
            DropColumn("dbo.Activoes", "isPrestado");
            DropColumn("dbo.Activoes", "contabilidadActivo");
            DropColumn("dbo.Activoes", "observacionesActivo");
            DropColumn("dbo.Activoes", "mmcUnidades");
            DropColumn("dbo.Activoes", "unidadesActivo");
            DropColumn("dbo.Activoes", "descripcionActivo");
            DropColumn("dbo.Activoes", "codigoActivo");
            DropColumn("dbo.Activoes", "partidaNumActivo");
            DropTable("dbo.AdquisicionDeActivoes");
            DropTable("dbo.CategoriaDeActivoes");
            RenameColumn(table: "dbo.PrestamoActivoes", name: "Activo_idActivo", newName: "idActivo");
            CreateIndex("dbo.PrestamoActivoes", "idActivo");
            CreateIndex("dbo.Inventarios", "departamentoID");
            CreateIndex("dbo.Activoes", "inventarioID");
            AddForeignKey("dbo.PrestamoActivoes", "idActivo", "dbo.Activoes", "idActivo", cascadeDelete: true);
            AddForeignKey("dbo.Inventarios", "departamentoID", "dbo.Departamentoes", "departamentoID");
            AddForeignKey("dbo.Activoes", "inventarioID", "dbo.Inventarios", "inventarioID", cascadeDelete: true);
        }
    }
}
