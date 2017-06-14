namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class liquidacion_added_ListaDeIngresosDeCosecha : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "liquidacionDeCosechaID", c => c.Int());
            CreateIndex("dbo.MovimientoFinancieroes", "liquidacionDeCosechaID");
            AddForeignKey("dbo.MovimientoFinancieroes", "liquidacionDeCosechaID", "dbo.MovimientoFinancieroes", "idMovimiento");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MovimientoFinancieroes", "liquidacionDeCosechaID", "dbo.MovimientoFinancieroes");
            DropIndex("dbo.MovimientoFinancieroes", new[] { "liquidacionDeCosechaID" });
            DropColumn("dbo.MovimientoFinancieroes", "liquidacionDeCosechaID");
        }
    }
}
