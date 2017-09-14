namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nuevatabla_retencionesDeCheques : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RetencionCheques",
                c => new
                    {
                        chequeID = c.Int(nullable: false, identity: true),
                        numCheque = c.String(nullable: false),
                        fecha = c.DateTime(nullable: false),
                        retencionID = c.Int(nullable: false),
                        monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.chequeID)
                .ForeignKey("dbo.MovimientoFinancieroes", t => t.retencionID, cascadeDelete: true)
                .Index(t => t.retencionID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RetencionCheques", "retencionID", "dbo.MovimientoFinancieroes");
            DropIndex("dbo.RetencionCheques", new[] { "retencionID" });
            DropTable("dbo.RetencionCheques");
        }
    }
}
