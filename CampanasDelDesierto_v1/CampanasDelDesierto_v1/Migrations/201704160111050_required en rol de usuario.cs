namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requiredenroldeusuario : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "Rol", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Rol", c => c.String());
        }
    }
}
