namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cambios_Modelo_Activo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activoes", "codigoActivo", c => c.String());
            AddColumn("dbo.Activoes", "descripcionActivo", c => c.String());
            DropColumn("dbo.Activoes", "estadoActivo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Activoes", "estadoActivo", c => c.String());
            DropColumn("dbo.Activoes", "descripcionActivo");
            DropColumn("dbo.Activoes", "codigoActivo");
        }
    }
}
