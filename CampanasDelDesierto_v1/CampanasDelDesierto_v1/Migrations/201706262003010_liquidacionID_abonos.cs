namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class liquidacionID_abonos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "abonoEnliquidacionID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "abonoEnliquidacionID");
        }
    }
}
