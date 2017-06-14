namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rol_Identity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "rol", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "rol");
        }
    }
}
