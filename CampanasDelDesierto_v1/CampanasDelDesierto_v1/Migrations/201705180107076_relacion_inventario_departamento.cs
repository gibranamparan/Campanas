namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relacion_inventario_departamento : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Inventarios", "idSucursal", "dbo.Sucursals");
            DropIndex("dbo.Inventarios", new[] { "idSucursal" });
            AddColumn("dbo.Inventarios", "departamentoID", c => c.Int());
            AddColumn("dbo.Departamentoes", "domicilio", c => c.String());
            CreateIndex("dbo.Inventarios", "departamentoID");
            AddForeignKey("dbo.Inventarios", "departamentoID", "dbo.Departamentoes", "departamentoID");
            DropColumn("dbo.Inventarios", "idSucursal");
            DropColumn("dbo.Sucursals", "domicilioSucursal");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sucursals", "domicilioSucursal", c => c.String());
            AddColumn("dbo.Inventarios", "idSucursal", c => c.Int(nullable: false));
            DropForeignKey("dbo.Inventarios", "departamentoID", "dbo.Departamentoes");
            DropIndex("dbo.Inventarios", new[] { "departamentoID" });
            DropColumn("dbo.Departamentoes", "domicilio");
            DropColumn("dbo.Inventarios", "departamentoID");
            CreateIndex("dbo.Inventarios", "idSucursal");
            AddForeignKey("dbo.Inventarios", "idSucursal", "dbo.Sucursals", "idSucursal", cascadeDelete: true);
        }
    }
}
