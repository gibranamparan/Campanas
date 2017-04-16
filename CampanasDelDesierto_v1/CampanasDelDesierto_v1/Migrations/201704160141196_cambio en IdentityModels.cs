namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambioenIdentityModels : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "rol");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "rol", c => c.String(nullable: false));
        }
    }
}
