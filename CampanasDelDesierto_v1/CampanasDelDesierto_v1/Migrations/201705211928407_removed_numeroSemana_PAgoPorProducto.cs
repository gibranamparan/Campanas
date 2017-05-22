namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removed_numeroSemana_PAgoPorProducto : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MovimientoFinancieroes", "numeroSemana");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "numeroSemana", c => c.Int());
        }
    }
}
