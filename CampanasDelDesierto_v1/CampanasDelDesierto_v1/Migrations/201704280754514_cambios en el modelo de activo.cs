namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambiosenelmodelodeactivo : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Activoes", "Inventario_inventarioID", "dbo.Inventarios");
            DropIndex("dbo.Activoes", new[] { "Inventario_inventarioID" });
            RenameColumn(table: "dbo.Activoes", name: "Inventario_inventarioID", newName: "inventarioID");
            AlterColumn("dbo.Activoes", "inventarioID", c => c.Int(nullable: false));
            CreateIndex("dbo.Activoes", "inventarioID");
            AddForeignKey("dbo.Activoes", "inventarioID", "dbo.Inventarios", "inventarioID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Activoes", "inventarioID", "dbo.Inventarios");
            DropIndex("dbo.Activoes", new[] { "inventarioID" });
            AlterColumn("dbo.Activoes", "inventarioID", c => c.Int());
            RenameColumn(table: "dbo.Activoes", name: "inventarioID", newName: "Inventario_inventarioID");
            CreateIndex("dbo.Activoes", "Inventario_inventarioID");
            AddForeignKey("dbo.Activoes", "Inventario_inventarioID", "dbo.Inventarios", "inventarioID");
        }
    }
}
