namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edicion_EmisionDeCheques : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MovimientoFinancieroes", "abonoAnticipo");
            DropColumn("dbo.MovimientoFinancieroes", "garantiaLimpieza");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "garantiaLimpieza", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.MovimientoFinancieroes", "abonoAnticipo", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
