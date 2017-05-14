namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recepcionDeProducto_NulleableTemporadaID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RecepcionDeProductoes", "TemporadaDeCosechaID", "dbo.TemporadaDeCosechas");
            DropIndex("dbo.RecepcionDeProductoes", new[] { "TemporadaDeCosechaID" });
            AlterColumn("dbo.RecepcionDeProductoes", "TemporadaDeCosechaID", c => c.Int());
            CreateIndex("dbo.RecepcionDeProductoes", "TemporadaDeCosechaID");
            AddForeignKey("dbo.RecepcionDeProductoes", "TemporadaDeCosechaID", "dbo.TemporadaDeCosechas", "TemporadaDeCosechaID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecepcionDeProductoes", "TemporadaDeCosechaID", "dbo.TemporadaDeCosechas");
            DropIndex("dbo.RecepcionDeProductoes", new[] { "TemporadaDeCosechaID" });
            AlterColumn("dbo.RecepcionDeProductoes", "TemporadaDeCosechaID", c => c.Int(nullable: false));
            CreateIndex("dbo.RecepcionDeProductoes", "TemporadaDeCosechaID");
            AddForeignKey("dbo.RecepcionDeProductoes", "TemporadaDeCosechaID", "dbo.TemporadaDeCosechas", "TemporadaDeCosechaID", cascadeDelete: true);
        }
    }
}
