namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cargosDeInteres : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "cheque", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "cheque1", newName: "cheque");
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "__mig_tmp__0", newName: "cheque1");
            CreateTable(
                "dbo.CargoDeInteres",
                c => new
                    {
                        cargoDeInteresID = c.Int(nullable: false, identity: true),
                        monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                        fecha = c.DateTime(nullable: false),
                        anticipoID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.cargoDeInteresID)
                .ForeignKey("dbo.MovimientoFinancieroes", t => t.anticipoID, cascadeDelete: true)
                .Index(t => t.anticipoID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CargoDeInteres", "anticipoID", "dbo.MovimientoFinancieroes");
            DropIndex("dbo.CargoDeInteres", new[] { "anticipoID" });
            DropTable("dbo.CargoDeInteres");
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "cheque1", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "cheque", newName: "cheque1");
            RenameColumn(table: "dbo.MovimientoFinancieroes", name: "__mig_tmp__0", newName: "cheque");
        }
    }
}
