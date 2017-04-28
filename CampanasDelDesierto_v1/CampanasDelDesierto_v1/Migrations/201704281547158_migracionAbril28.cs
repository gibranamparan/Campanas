namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migracionAbril28 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Activoes", "inventarioID", "dbo.Inventarios");
            DropIndex("dbo.Activoes", new[] { "inventarioID" });
            RenameColumn(table: "dbo.Activoes", name: "inventarioID", newName: "Inventario_inventarioID");
            AlterColumn("dbo.Activoes", "Inventario_inventarioID", c => c.Int());
            CreateIndex("dbo.Activoes", "Inventario_inventarioID");
            AddForeignKey("dbo.Activoes", "Inventario_inventarioID", "dbo.Inventarios", "inventarioID");
            DropColumn("dbo.Inventarios", "cantidad");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Inventarios", "cantidad", c => c.Int(nullable: false));
            DropForeignKey("dbo.Activoes", "Inventario_inventarioID", "dbo.Inventarios");
            DropIndex("dbo.Activoes", new[] { "Inventario_inventarioID" });
            AlterColumn("dbo.Activoes", "Inventario_inventarioID", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Activoes", name: "Inventario_inventarioID", newName: "inventarioID");
            CreateIndex("dbo.Activoes", "inventarioID");
            AddForeignKey("dbo.Activoes", "inventarioID", "dbo.Inventarios", "inventarioID", cascadeDelete: true);
        }
    }
}
