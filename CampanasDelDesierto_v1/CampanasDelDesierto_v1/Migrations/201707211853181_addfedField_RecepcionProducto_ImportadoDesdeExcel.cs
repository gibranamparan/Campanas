namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfedField_RecepcionProducto_ImportadoDesdeExcel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RecepcionDeProductoes", "importadoDesdeExcel", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RecepcionDeProductoes", "importadoDesdeExcel");
        }
    }
}
