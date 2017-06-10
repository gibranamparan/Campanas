namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class retencionesLIst_in_LiquidacionSemanal : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MovimientoFinancieroes", "rentecionEjidalID", "dbo.MovimientoFinancieroes");
            DropForeignKey("dbo.MovimientoFinancieroes", "retencionOtroID", "dbo.MovimientoFinancieroes");
            DropIndex("dbo.MovimientoFinancieroes", new[] { "rentecionEjidalID" });
            DropIndex("dbo.MovimientoFinancieroes", new[] { "retencionOtroID" });
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "garantiaLimpiezaID", newName: "liquidacionID");
            RenameIndex(table: "dbo.MovimientoFinancieroes", name: "IX_garantiaLimpiezaID", newName: "IX_liquidacionID");
            DropColumn("dbo.MovimientoFinancieroes", "rentecionEjidalID");
            DropColumn("dbo.MovimientoFinancieroes", "retencionOtroID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "retencionOtroID", c => c.Int());
            AddColumn("dbo.MovimientoFinancieroes", "rentecionEjidalID", c => c.Int());
            RenameIndex(table: "dbo.MovimientoFinancieroes", name: "IX_liquidacionID", newName: "IX_garantiaLimpiezaID");
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "liquidacionID", newName: "garantiaLimpiezaID");
            CreateIndex("dbo.MovimientoFinancieroes", "retencionOtroID");
            CreateIndex("dbo.MovimientoFinancieroes", "rentecionEjidalID");
            AddForeignKey("dbo.MovimientoFinancieroes", "retencionOtroID", "dbo.MovimientoFinancieroes", "idMovimiento");
            AddForeignKey("dbo.MovimientoFinancieroes", "rentecionEjidalID", "dbo.MovimientoFinancieroes", "idMovimiento");
        }
    }
}
