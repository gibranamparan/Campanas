namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nuevaTabla_RecepcionAceituna : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecepcionDeProductoes",
                c => new
                    {
                        numRecibo = c.Int(nullable: false, identity: true),
                        numProductor = c.String(),
                        nombreProductor = c.String(),
                        cantidadTonsProd1 = c.Double(nullable: false),
                        cantidadTonsProd2 = c.Double(nullable: false),
                        cantidadTonsProd3 = c.Double(nullable: false),
                        TemporadaDeCosechaID = c.Int(nullable: false),
                        idProductor = c.Int(),
                    })
                .PrimaryKey(t => t.numRecibo)
                .ForeignKey("dbo.Productors", t => t.idProductor)
                .ForeignKey("dbo.TemporadaDeCosechas", t => t.TemporadaDeCosechaID, cascadeDelete: true)
                .Index(t => t.TemporadaDeCosechaID)
                .Index(t => t.idProductor);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecepcionDeProductoes", "TemporadaDeCosechaID", "dbo.TemporadaDeCosechas");
            DropForeignKey("dbo.RecepcionDeProductoes", "idProductor", "dbo.Productors");
            DropIndex("dbo.RecepcionDeProductoes", new[] { "idProductor" });
            DropIndex("dbo.RecepcionDeProductoes", new[] { "TemporadaDeCosechaID" });
            DropTable("dbo.RecepcionDeProductoes");
        }
    }
}
