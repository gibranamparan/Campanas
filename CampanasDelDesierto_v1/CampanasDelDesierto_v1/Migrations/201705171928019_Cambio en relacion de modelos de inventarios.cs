namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cambioenrelaciondemodelosdeinventarios : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Inventarios", "idSucursal", "dbo.Sucursals");
            DropIndex("dbo.Inventarios", new[] { "idSucursal" });
            AddColumn("dbo.Inventarios", "departamentoID", c => c.Int(nullable: false));
            CreateIndex("dbo.Inventarios", "departamentoID");
            AddForeignKey("dbo.Inventarios", "departamentoID", "dbo.Departamentoes", "departamentoID", cascadeDelete: true);
            DropColumn("dbo.Inventarios", "idSucursal");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Inventarios", "idSucursal", c => c.Int(nullable: false));
            DropForeignKey("dbo.Inventarios", "departamentoID", "dbo.Departamentoes");
            DropIndex("dbo.Inventarios", new[] { "departamentoID" });
            DropColumn("dbo.Inventarios", "departamentoID");
            CreateIndex("dbo.Inventarios", "idSucursal");
            AddForeignKey("dbo.Inventarios", "idSucursal", "dbo.Sucursals", "idSucursal", cascadeDelete: true);
        }
    }
}
