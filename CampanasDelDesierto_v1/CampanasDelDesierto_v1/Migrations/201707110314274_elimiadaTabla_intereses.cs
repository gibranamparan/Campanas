namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class elimiadaTabla_intereses : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CargoDeInteres", "anticipoID", "dbo.MovimientoFinancieroes");
            DropIndex("dbo.CargoDeInteres", new[] { "anticipoID" });
            DropTable("dbo.CargoDeInteres");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CargoDeInteres",
                c => new
                    {
                        cargoDeInteresID = c.Int(nullable: false, identity: true),
                        monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                        fecha = c.DateTime(nullable: false),
                        anticipoID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.cargoDeInteresID);
            
            CreateIndex("dbo.CargoDeInteres", "anticipoID");
            AddForeignKey("dbo.CargoDeInteres", "anticipoID", "dbo.MovimientoFinancieroes", "idMovimiento", cascadeDelete: true);
        }
    }
}
