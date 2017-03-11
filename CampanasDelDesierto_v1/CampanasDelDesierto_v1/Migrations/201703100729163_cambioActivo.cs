namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambioActivo : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MovimientoFinancieroes", "activo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "activo", c => c.String());
        }
    }
}
