namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Eliminacion_Datos_AdminDepartamentos : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "nombre", c => c.String(defaultValue:""));
            AlterColumn("dbo.AspNetUsers", "apellidoPaterno", c => c.String());
            AlterColumn("dbo.AspNetUsers", "apellidoMaterno", c => c.String());
            AlterColumn("dbo.AspNetUsers", "Email", c => c.String(maxLength: 256));
            DropColumn("dbo.AspNetUsers", "userID");
            DropColumn("dbo.AspNetUsers", "Password");
            DropColumn("dbo.AspNetUsers", "ConfirmPassword");
            DropColumn("dbo.AspNetUsers", "registerAsAdmin");
            DropColumn("dbo.AspNetUsers", "hash");
            DropColumn("dbo.AspNetUsers", "stamp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "stamp", c => c.String());
            AddColumn("dbo.AspNetUsers", "hash", c => c.String());
            AddColumn("dbo.AspNetUsers", "registerAsAdmin", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "ConfirmPassword", c => c.String());
            AddColumn("dbo.AspNetUsers", "Password", c => c.String(maxLength: 100));
            AddColumn("dbo.AspNetUsers", "userID", c => c.String());
            AlterColumn("dbo.AspNetUsers", "Email", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.AspNetUsers", "apellidoMaterno", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "apellidoPaterno", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "nombre", c => c.String(nullable: false));
        }
    }
}
