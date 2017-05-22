namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removed_nota_PrestamoYAbonoCapitals : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MovimientoFinancieroes", "nota");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "nota", c => c.String());
        }
    }
}
