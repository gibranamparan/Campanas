namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modificaciones : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "abonoAnticipo", c => c.Int());
            DropColumn("dbo.MovimientoFinancieroes", "MyProperty");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "MyProperty", c => c.Int());
            DropColumn("dbo.MovimientoFinancieroes", "abonoAnticipo");
        }
    }
}
