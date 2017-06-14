namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cambios_IdentityModels : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "nombre", c => c.String());
            AlterColumn("dbo.AspNetUsers", "apellidoPaterno", c => c.String());
            AlterColumn("dbo.AspNetUsers", "apellidoMaterno", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "apellidoMaterno", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "apellidoPaterno", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "nombre", c => c.String(nullable: false));
        }
    }
}
