namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fecha_semana_RecepcionDeProducto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RecepcionDeProductoes", "fecha", c => c.DateTime(nullable: false));
            AddColumn("dbo.RecepcionDeProductoes", "semana", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RecepcionDeProductoes", "semana");
            DropColumn("dbo.RecepcionDeProductoes", "fecha");
        }
    }
}
