namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class presta_abono_relacionPar : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Prestamo_Abono",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        prestamoID = c.Int(),
                        abonoID = c.Int(),
                        monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.MovimientoFinancieroes", t => t.prestamoID)
                .ForeignKey("dbo.MovimientoFinancieroes", t => t.abonoID)
                .Index(t => t.prestamoID)
                .Index(t => t.abonoID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Prestamo_Abono", "abonoID", "dbo.MovimientoFinancieroes");
            DropForeignKey("dbo.Prestamo_Abono", "prestamoID", "dbo.MovimientoFinancieroes");
            DropIndex("dbo.Prestamo_Abono", new[] { "abonoID" });
            DropIndex("dbo.Prestamo_Abono", new[] { "prestamoID" });
            DropTable("dbo.Prestamo_Abono");
        }
    }
}
