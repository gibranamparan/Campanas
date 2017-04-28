namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class atributocantidadeninventario : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inventarios", "cantidad", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Inventarios", "cantidad");
        }
    }
}
