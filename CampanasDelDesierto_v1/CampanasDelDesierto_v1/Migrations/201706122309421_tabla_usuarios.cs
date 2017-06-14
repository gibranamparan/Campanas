namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tabla_usuarios : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "nombre", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "apellidoPaterno", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "apellidoMaterno", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "apellidoMaterno");
            DropColumn("dbo.AspNetUsers", "apellidoPaterno");
            DropColumn("dbo.AspNetUsers", "nombre");
        }
    }
}
