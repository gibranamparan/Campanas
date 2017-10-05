namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cambio_Modelo_PrestamoActivo : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PrestamoActivoes", "idActivo", "dbo.Activoes");
            DropIndex("dbo.PrestamoActivoes", new[] { "idActivo" });
            RenameColumn(table: "dbo.PrestamoActivoes", name: "idActivo", newName: "Activo_idActivo");
            AlterColumn("dbo.PrestamoActivoes", "Activo_idActivo", c => c.Int());
            CreateIndex("dbo.PrestamoActivoes", "Activo_idActivo");
            AddForeignKey("dbo.PrestamoActivoes", "Activo_idActivo", "dbo.Activoes", "idActivo");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PrestamoActivoes", "Activo_idActivo", "dbo.Activoes");
            DropIndex("dbo.PrestamoActivoes", new[] { "Activo_idActivo" });
            AlterColumn("dbo.PrestamoActivoes", "Activo_idActivo", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.PrestamoActivoes", name: "Activo_idActivo", newName: "idActivo");
            CreateIndex("dbo.PrestamoActivoes", "idActivo");
            AddForeignKey("dbo.PrestamoActivoes", "idActivo", "dbo.Activoes", "idActivo", cascadeDelete: true);
        }
    }
}
