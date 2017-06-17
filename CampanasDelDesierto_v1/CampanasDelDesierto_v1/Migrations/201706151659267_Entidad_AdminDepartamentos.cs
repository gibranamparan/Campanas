namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Entidad_AdminDepartamentos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "departamentoID", c => c.Int());
            AddColumn("dbo.AspNetUsers", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.AspNetUsers", "departamentoID");
            AddForeignKey("dbo.AspNetUsers", "departamentoID", "dbo.Departamentoes", "departamentoID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "departamentoID", "dbo.Departamentoes");
            DropIndex("dbo.AspNetUsers", new[] { "departamentoID" });
            DropColumn("dbo.AspNetUsers", "Discriminator");
            DropColumn("dbo.AspNetUsers", "departamentoID");
        }
    }
}
