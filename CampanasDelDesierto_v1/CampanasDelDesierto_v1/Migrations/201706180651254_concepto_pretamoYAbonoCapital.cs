namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class concepto_pretamoYAbonoCapital : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "conceptoCapital", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "conceptoCapital");
        }
    }
}
