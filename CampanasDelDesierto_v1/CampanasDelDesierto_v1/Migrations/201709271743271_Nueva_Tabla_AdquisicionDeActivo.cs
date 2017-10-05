namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nueva_Tabla_AdquisicionDeActivo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdquisicionDeActivoes",
                c => new
                    {
                        AdquirirActivosID = c.Int(nullable: false, identity: true),
                        cantidadActivo = c.Int(nullable: false),
                        idActivo = c.Int(),
                        idPrestamoActivo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AdquirirActivosID)
                .ForeignKey("dbo.Activoes", t => t.idActivo)
                .ForeignKey("dbo.PrestamoActivoes", t => t.idPrestamoActivo, cascadeDelete: true)
                .Index(t => t.idActivo)
                .Index(t => t.idPrestamoActivo);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdquisicionDeActivoes", "idPrestamoActivo", "dbo.PrestamoActivoes");
            DropForeignKey("dbo.AdquisicionDeActivoes", "idActivo", "dbo.Activoes");
            DropIndex("dbo.AdquisicionDeActivoes", new[] { "idPrestamoActivo" });
            DropIndex("dbo.AdquisicionDeActivoes", new[] { "idActivo" });
            DropTable("dbo.AdquisicionDeActivoes");
        }
    }
}
