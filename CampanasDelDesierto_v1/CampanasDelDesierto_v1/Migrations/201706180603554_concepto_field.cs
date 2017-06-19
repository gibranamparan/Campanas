namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class concepto_field : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MovimientoFinancieroes", "concepto");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "concepto", c => c.String());
        }
    }
}
