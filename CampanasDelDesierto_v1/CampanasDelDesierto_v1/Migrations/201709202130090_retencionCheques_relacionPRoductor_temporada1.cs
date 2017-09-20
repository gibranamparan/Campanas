namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class retencionCheques_relacionPRoductor_temporada1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "semana", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "semana1", newName: "semana");
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "__mig_tmp__0", newName: "semana1");
            CreateTable(
                "dbo.RetencionCheques",
                c => new
                    {
                        chequeID = c.Int(nullable: false, identity: true),
                        numCheque = c.String(nullable: false),
                        fecha = c.DateTime(nullable: false),
                        productorID = c.Int(nullable: false),
                        temporadaID = c.Int(nullable: false),
                        monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                        tipoDeDeduccion = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.chequeID)
                .ForeignKey("dbo.Productors", t => t.productorID, cascadeDelete: true)
                .ForeignKey("dbo.TemporadaDeCosechas", t => t.temporadaID, cascadeDelete: true)
                .Index(t => t.productorID)
                .Index(t => t.temporadaID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RetencionCheques", "temporadaID", "dbo.TemporadaDeCosechas");
            DropForeignKey("dbo.RetencionCheques", "productorID", "dbo.Productors");
            DropIndex("dbo.RetencionCheques", new[] { "temporadaID" });
            DropIndex("dbo.RetencionCheques", new[] { "productorID" });
            DropTable("dbo.RetencionCheques");
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "semana1", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "semana", newName: "semana1");
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "__mig_tmp__0", newName: "semana");
        }
    }
}
