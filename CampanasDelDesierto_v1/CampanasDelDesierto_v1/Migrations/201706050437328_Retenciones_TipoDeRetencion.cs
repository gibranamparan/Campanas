namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Retenciones_TipoDeRetencion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "tipoDeDeduccion", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "tipoDeDeduccion");
        }
    }
}
