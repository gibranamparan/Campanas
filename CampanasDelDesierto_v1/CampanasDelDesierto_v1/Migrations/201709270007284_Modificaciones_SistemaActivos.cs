namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modificaciones_SistemaActivos : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Activoes", name: "Departamento_departamentoID", newName: "departamentoID");
            RenameIndex(table: "dbo.Activoes", name: "IX_Departamento_departamentoID", newName: "IX_departamentoID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Activoes", name: "IX_departamentoID", newName: "IX_Departamento_departamentoID");
            RenameColumn(table: "dbo.Activoes", name: "departamentoID", newName: "Departamento_departamentoID");
        }
    }
}
