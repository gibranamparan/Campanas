namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_field_bajamanzanitaorganica : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TemporadaDeCosechas", "precioProducto8", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TemporadaDeCosechas", "precioProducto8");
        }
    }
}
