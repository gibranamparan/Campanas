namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recepcionDeProducto_RecepcionID : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.RecepcionDeProductoes");
            DropColumn("dbo.RecepcionDeProductoes", "numRecibo");
            AddColumn("dbo.RecepcionDeProductoes", "recepcionID", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.RecepcionDeProductoes", "numeroRecibo", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.RecepcionDeProductoes", "recepcionID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.RecepcionDeProductoes");
            DropColumn("dbo.RecepcionDeProductoes", "numeroRecibo");
            DropColumn("dbo.RecepcionDeProductoes", "recepcionID");
            AddColumn("dbo.RecepcionDeProductoes", "numRecibo", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.RecepcionDeProductoes", "numRecibo");
        }
    }
}
