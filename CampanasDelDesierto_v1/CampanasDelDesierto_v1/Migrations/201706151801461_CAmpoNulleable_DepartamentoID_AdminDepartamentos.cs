namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CAmpoNulleable_DepartamentoID_AdminDepartamentos : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "departamentoID", "dbo.Departamentoes");
            AddForeignKey("dbo.AspNetUsers", "departamentoID", "dbo.Departamentoes", "departamentoID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "departamentoID", "dbo.Departamentoes");
            AddForeignKey("dbo.AspNetUsers", "departamentoID", "dbo.Departamentoes", "departamentoID", cascadeDelete: true);
        }
    }
}
