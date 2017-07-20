namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addField_FechaApagar_VentaACredito : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "fechaPagar1", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "fechaPagar1");
        }
    }
}
