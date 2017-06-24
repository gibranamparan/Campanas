namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambioNombreAtributo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productors", "Desactivado", c => c.Boolean(nullable: false));
            DropColumn("dbo.Productors", "Activado");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Productors", "Activado", c => c.Boolean(nullable: false));
            DropColumn("dbo.Productors", "Desactivado");
        }
    }
}
