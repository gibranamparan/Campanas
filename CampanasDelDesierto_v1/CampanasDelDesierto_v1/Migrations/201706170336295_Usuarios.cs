namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Usuarios : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "userID", c => c.String());
            AddColumn("dbo.AspNetUsers", "Password", c => c.String(maxLength: 100));
            AddColumn("dbo.AspNetUsers", "ConfirmPassword", c => c.String());
            AddColumn("dbo.AspNetUsers", "registerAsAdmin", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "hash", c => c.String());
            AddColumn("dbo.AspNetUsers", "stamp", c => c.String());
            AlterColumn("dbo.AspNetUsers", "nombre", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "apellidoPaterno", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "apellidoMaterno", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "Email", c => c.String(nullable: false, maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Email", c => c.String(maxLength: 256));
            AlterColumn("dbo.AspNetUsers", "apellidoMaterno", c => c.String());
            AlterColumn("dbo.AspNetUsers", "apellidoPaterno", c => c.String());
            AlterColumn("dbo.AspNetUsers", "nombre", c => c.String());
            DropColumn("dbo.AspNetUsers", "stamp");
            DropColumn("dbo.AspNetUsers", "hash");
            DropColumn("dbo.AspNetUsers", "registerAsAdmin");
            DropColumn("dbo.AspNetUsers", "ConfirmPassword");
            DropColumn("dbo.AspNetUsers", "Password");
            DropColumn("dbo.AspNetUsers", "userID");
        }
    }
}
