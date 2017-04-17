namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class campoAgregado_Producto_isArbolAceituna : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productoes", "isArbolAceituna", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Productoes", "isArbolAceituna");
        }
    }
}
