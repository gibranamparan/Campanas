namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oblizaPrecio_Productores : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productors", "precioProducto3", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Productors", "precioProducto3");
        }
    }
}
