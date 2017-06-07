namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class intoToString_pozo_PrestamoYAbonoCapital : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MovimientoFinancieroes", "pozo", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MovimientoFinancieroes", "pozo", c => c.Int());
        }
    }
}
