namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedFields_Retenciones_LiquidacionSemanal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "garantiaLimpiezaID", c => c.Int());
            AddColumn("dbo.MovimientoFinancieroes", "rentecionEjidalID", c => c.Int());
            AddColumn("dbo.MovimientoFinancieroes", "abonoAnticipoID", c => c.Int());
            AddColumn("dbo.MovimientoFinancieroes", "retencionOtroID", c => c.Int());
            CreateIndex("dbo.MovimientoFinancieroes", "garantiaLimpiezaID");
            CreateIndex("dbo.MovimientoFinancieroes", "rentecionEjidalID");
            CreateIndex("dbo.MovimientoFinancieroes", "abonoAnticipoID");
            CreateIndex("dbo.MovimientoFinancieroes", "retencionOtroID");
            AddForeignKey("dbo.MovimientoFinancieroes", "abonoAnticipoID", "dbo.MovimientoFinancieroes", "idMovimiento");
            AddForeignKey("dbo.MovimientoFinancieroes", "garantiaLimpiezaID", "dbo.MovimientoFinancieroes", "idMovimiento");
            AddForeignKey("dbo.MovimientoFinancieroes", "rentecionEjidalID", "dbo.MovimientoFinancieroes", "idMovimiento");
            AddForeignKey("dbo.MovimientoFinancieroes", "retencionOtroID", "dbo.MovimientoFinancieroes", "idMovimiento");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MovimientoFinancieroes", "retencionOtroID", "dbo.MovimientoFinancieroes");
            DropForeignKey("dbo.MovimientoFinancieroes", "rentecionEjidalID", "dbo.MovimientoFinancieroes");
            DropForeignKey("dbo.MovimientoFinancieroes", "garantiaLimpiezaID", "dbo.MovimientoFinancieroes");
            DropForeignKey("dbo.MovimientoFinancieroes", "abonoAnticipoID", "dbo.MovimientoFinancieroes");
            DropIndex("dbo.MovimientoFinancieroes", new[] { "retencionOtroID" });
            DropIndex("dbo.MovimientoFinancieroes", new[] { "abonoAnticipoID" });
            DropIndex("dbo.MovimientoFinancieroes", new[] { "rentecionEjidalID" });
            DropIndex("dbo.MovimientoFinancieroes", new[] { "garantiaLimpiezaID" });
            DropColumn("dbo.MovimientoFinancieroes", "retencionOtroID");
            DropColumn("dbo.MovimientoFinancieroes", "abonoAnticipoID");
            DropColumn("dbo.MovimientoFinancieroes", "rentecionEjidalID");
            DropColumn("dbo.MovimientoFinancieroes", "garantiaLimpiezaID");
        }
    }
}
