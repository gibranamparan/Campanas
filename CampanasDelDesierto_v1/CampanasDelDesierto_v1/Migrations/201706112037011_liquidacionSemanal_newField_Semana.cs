namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class liquidacionSemanal_newField_Semana : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "semana", c => c.Int());
            AddColumn("dbo.MovimientoFinancieroes", "semana1", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "semana1");
            DropColumn("dbo.MovimientoFinancieroes", "semana");
        }
    }
}
