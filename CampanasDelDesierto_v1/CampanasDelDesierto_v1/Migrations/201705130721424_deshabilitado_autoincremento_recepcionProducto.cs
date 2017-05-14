namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deshabilitado_autoincremento_recepcionProducto : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.RecepcionDeProductoes");
            AlterColumn("dbo.RecepcionDeProductoes", "numRecibo", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.RecepcionDeProductoes", "numRecibo");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.RecepcionDeProductoes");
            AlterColumn("dbo.RecepcionDeProductoes", "numRecibo", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.RecepcionDeProductoes", "numRecibo");
        }
    }
}
