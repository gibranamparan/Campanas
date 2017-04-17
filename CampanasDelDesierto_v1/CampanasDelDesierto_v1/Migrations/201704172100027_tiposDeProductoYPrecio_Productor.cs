namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tiposDeProductoYPrecio_Productor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productors", "precioProducto1", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Productors", "precioProducto2", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Productors", "precioProductoOtro", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Productors", "precioProductoOtro");
            DropColumn("dbo.Productors", "precioProducto2");
            DropColumn("dbo.Productors", "precioProducto1");
        }
    }
}
