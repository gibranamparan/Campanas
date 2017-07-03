namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class liquidacion_abonoArboles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "abonoArbolesID", c => c.Int());
            CreateIndex("dbo.MovimientoFinancieroes", "abonoArbolesID");
            AddForeignKey("dbo.MovimientoFinancieroes", "abonoArbolesID", "dbo.MovimientoFinancieroes", "idMovimiento");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MovimientoFinancieroes", "abonoArbolesID", "dbo.MovimientoFinancieroes");
            DropIndex("dbo.MovimientoFinancieroes", new[] { "abonoArbolesID" });
            DropColumn("dbo.MovimientoFinancieroes", "abonoArbolesID");
        }
    }
}
