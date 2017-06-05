namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class concepto_A_TipoDeMovimiento : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "tipoDeMovimientoDeCapital", c => c.String());
            DropColumn("dbo.MovimientoFinancieroes", "concepto");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "concepto", c => c.String());
            DropColumn("dbo.MovimientoFinancieroes", "tipoDeMovimientoDeCapital");
        }
    }
}
