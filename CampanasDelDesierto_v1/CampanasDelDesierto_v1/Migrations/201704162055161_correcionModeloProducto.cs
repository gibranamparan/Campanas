namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class correcionModeloProducto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productoes", "descripcion", c => c.String());
            DropColumn("dbo.Productoes", "fecha");
            DropColumn("dbo.Productoes", "concepto");
            DropColumn("dbo.Productoes", "pagare");
            DropColumn("dbo.Productoes", "ordenDeCompra");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Productoes", "ordenDeCompra", c => c.String());
            AddColumn("dbo.Productoes", "pagare", c => c.String());
            AddColumn("dbo.Productoes", "concepto", c => c.String());
            AddColumn("dbo.Productoes", "fecha", c => c.DateTime(nullable: false));
            DropColumn("dbo.Productoes", "descripcion");
        }
    }
}
