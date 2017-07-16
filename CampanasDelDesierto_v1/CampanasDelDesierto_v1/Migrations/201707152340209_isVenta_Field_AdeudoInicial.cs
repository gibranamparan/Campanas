namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isVenta_Field_AdeudoInicial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "isVentas", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "isVentas");
        }
    }
}
