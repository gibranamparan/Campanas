namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedFiedl_liquidacionSemanal_timePeriod_precioDelDolar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "precioDelDolarEnLiquidacion", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("dbo.MovimientoFinancieroes", "semanaLiquidada_startDate", c => c.DateTime());
            AddColumn("dbo.MovimientoFinancieroes", "semanaLiquidada_endDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "semanaLiquidada_endDate");
            DropColumn("dbo.MovimientoFinancieroes", "semanaLiquidada_startDate");
            DropColumn("dbo.MovimientoFinancieroes", "precioDelDolarEnLiquidacion");
        }
    }
}
