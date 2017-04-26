namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class creacionTabla_TemporadaDeCosecha : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TemporadaDeCosechas",
                c => new
                    {
                        TemporadaDeCosechaID = c.Int(nullable: false, identity: true),
                        fechaInicio = c.DateTime(nullable: false),
                        fechaFin = c.DateTime(nullable: false),
                        precioProducto1 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        precioProducto2 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        precioProducto3 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        precioProductoOtro = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.TemporadaDeCosechaID);
            
            AddColumn("dbo.MovimientoFinancieroes", "TemporadaDeCosechaID", c => c.Int(nullable: false));
            CreateIndex("dbo.MovimientoFinancieroes", "TemporadaDeCosechaID");
            AddForeignKey("dbo.MovimientoFinancieroes", "TemporadaDeCosechaID", "dbo.TemporadaDeCosechas", "TemporadaDeCosechaID", cascadeDelete: true);
            DropColumn("dbo.Productors", "precioProducto1");
            DropColumn("dbo.Productors", "precioProducto2");
            DropColumn("dbo.Productors", "precioProducto3");
            DropColumn("dbo.Productors", "precioProductoOtro");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Productors", "precioProductoOtro", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Productors", "precioProducto3", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Productors", "precioProducto2", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Productors", "precioProducto1", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.MovimientoFinancieroes", "TemporadaDeCosechaID", "dbo.TemporadaDeCosechas");
            DropIndex("dbo.MovimientoFinancieroes", new[] { "TemporadaDeCosechaID" });
            DropColumn("dbo.MovimientoFinancieroes", "TemporadaDeCosechaID");
            DropTable("dbo.TemporadaDeCosechas");
        }
    }
}
