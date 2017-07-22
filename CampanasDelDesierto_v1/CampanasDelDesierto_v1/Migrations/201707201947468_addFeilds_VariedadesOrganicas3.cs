namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFeilds_VariedadesOrganicas3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TemporadaDeCosechas", "precioProducto9", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue:0));
            AddColumn("dbo.TemporadaDeCosechas", "precioProducto10", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0));
            AddColumn("dbo.TemporadaDeCosechas", "precioProducto11", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0));
            AddColumn("dbo.TemporadaDeCosechas", "precioProducto12", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TemporadaDeCosechas", "precioProducto12");
            DropColumn("dbo.TemporadaDeCosechas", "precioProducto11");
            DropColumn("dbo.TemporadaDeCosechas", "precioProducto10");
            DropColumn("dbo.TemporadaDeCosechas", "precioProducto9");
        }
    }
}
