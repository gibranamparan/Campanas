namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sincronizandoBaseDeDatos : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "liquidacionID", newName: "liquidacionSemanalID");
            RenameIndex(table: "dbo.MovimientoFinancieroes", name: "IX_liquidacionID", newName: "IX_liquidacionSemanalID");
            AddColumn("dbo.MovimientoFinancieroes", "semana", c => c.Int());
            AddColumn("dbo.MovimientoFinancieroes", "liquidacionDeCosechaID", c => c.Int());
            AddColumn("dbo.MovimientoFinancieroes", "semana1", c => c.Int());
            AddColumn("dbo.AspNetUsers", "nombre", c => c.String());
            AddColumn("dbo.AspNetUsers", "apellidoPaterno", c => c.String());
            AddColumn("dbo.AspNetUsers", "apellidoMaterno", c => c.String());
            AddColumn("dbo.AspNetUsers", "rol", c => c.String());
            CreateIndex("dbo.MovimientoFinancieroes", "liquidacionDeCosechaID");
            AddForeignKey("dbo.MovimientoFinancieroes", "liquidacionDeCosechaID", "dbo.MovimientoFinancieroes", "idMovimiento");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MovimientoFinancieroes", "liquidacionDeCosechaID", "dbo.MovimientoFinancieroes");
            DropIndex("dbo.MovimientoFinancieroes", new[] { "liquidacionDeCosechaID" });
            DropColumn("dbo.AspNetUsers", "rol");
            DropColumn("dbo.AspNetUsers", "apellidoMaterno");
            DropColumn("dbo.AspNetUsers", "apellidoPaterno");
            DropColumn("dbo.AspNetUsers", "nombre");
            DropColumn("dbo.MovimientoFinancieroes", "semana1");
            DropColumn("dbo.MovimientoFinancieroes", "liquidacionDeCosechaID");
            DropColumn("dbo.MovimientoFinancieroes", "semana");
            RenameIndex(table: "dbo.MovimientoFinancieroes", name: "IX_liquidacionSemanalID", newName: "IX_liquidacionID");
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "liquidacionSemanalID", newName: "liquidacionID");
        }
    }
}
