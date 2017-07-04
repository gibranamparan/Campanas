namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Orden_Compra_En_VentaACredito : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "ordenCompra", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "ordenCompra");
        }
    }
}
