namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class conceptosYPagara_VentaACredito : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "conceptoDeVenta", c => c.String());
            AddColumn("dbo.MovimientoFinancieroes", "pagareVenta", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "pagareVenta");
            DropColumn("dbo.MovimientoFinancieroes", "conceptoDeVenta");
        }
    }
}
