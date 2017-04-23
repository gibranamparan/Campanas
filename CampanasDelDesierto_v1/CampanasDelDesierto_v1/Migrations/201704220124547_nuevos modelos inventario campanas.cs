namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nuevosmodelosinventariocampanas : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Empleadoes", "idSucursal", "dbo.Sucursals");
            DropIndex("dbo.Empleadoes", new[] { "idSucursal" });
            CreateTable(
                "dbo.Inventarios",
                c => new
                    {
                        inventarioID = c.Int(nullable: false, identity: true),
                        nombreInventario = c.String(nullable: false),
                        idSucursal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.inventarioID)
                .ForeignKey("dbo.Sucursals", t => t.idSucursal, cascadeDelete: true)
                .Index(t => t.idSucursal);
            
            CreateTable(
                "dbo.Departamentoes",
                c => new
                    {
                        departamentoID = c.Int(nullable: false, identity: true),
                        nombreDepartamento = c.String(nullable: false),
                        Sucursal_idSucursal = c.Int(),
                    })
                .PrimaryKey(t => t.departamentoID)
                .ForeignKey("dbo.Sucursals", t => t.Sucursal_idSucursal)
                .Index(t => t.Sucursal_idSucursal);
            
            AddColumn("dbo.Activoes", "inventarioID", c => c.Int(nullable: false));
            AddColumn("dbo.Empleadoes", "departamentoID", c => c.Int(nullable: false));
            CreateIndex("dbo.Activoes", "inventarioID");
            CreateIndex("dbo.Empleadoes", "departamentoID");
            AddForeignKey("dbo.Activoes", "inventarioID", "dbo.Inventarios", "inventarioID", cascadeDelete: false);
            AddForeignKey("dbo.Empleadoes", "departamentoID", "dbo.Departamentoes", "departamentoID", cascadeDelete: false);
            DropColumn("dbo.Empleadoes", "idSucursal");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Empleadoes", "idSucursal", c => c.Int(nullable: false));
            DropForeignKey("dbo.Inventarios", "idSucursal", "dbo.Sucursals");
            DropForeignKey("dbo.Departamentoes", "Sucursal_idSucursal", "dbo.Sucursals");
            DropForeignKey("dbo.Empleadoes", "departamentoID", "dbo.Departamentoes");
            DropForeignKey("dbo.Activoes", "inventarioID", "dbo.Inventarios");
            DropIndex("dbo.Empleadoes", new[] { "departamentoID" });
            DropIndex("dbo.Departamentoes", new[] { "Sucursal_idSucursal" });
            DropIndex("dbo.Inventarios", new[] { "idSucursal" });
            DropIndex("dbo.Activoes", new[] { "inventarioID" });
            DropColumn("dbo.Empleadoes", "departamentoID");
            DropColumn("dbo.Activoes", "inventarioID");
            DropTable("dbo.Departamentoes");
            DropTable("dbo.Inventarios");
            CreateIndex("dbo.Empleadoes", "idSucursal");
            AddForeignKey("dbo.Empleadoes", "idSucursal", "dbo.Sucursals", "idSucursal", cascadeDelete: true);
        }
    }
}
