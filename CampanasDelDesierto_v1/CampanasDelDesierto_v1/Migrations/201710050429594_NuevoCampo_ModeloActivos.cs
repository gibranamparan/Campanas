namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NuevoCampo_ModeloActivos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activoes", "isPrestado", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Activoes", "isPrestado");
        }
    }
}
