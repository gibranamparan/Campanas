namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_field_VariedadOrganica1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TemporadaDeCosechas", "precioProducto7", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TemporadaDeCosechas", "precioProducto7");
        }
    }
}
