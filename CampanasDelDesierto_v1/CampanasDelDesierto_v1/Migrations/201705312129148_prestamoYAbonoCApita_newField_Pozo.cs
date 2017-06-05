namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prestamoYAbonoCApita_newField_Pozo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "pozo", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "pozo");
        }
    }
}
