namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class simplifcar_UnidadMedida : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productoes", "unidadMedida", c => c.String());
            DropColumn("dbo.Productoes", "UnidadMedida_nombre");
            DropColumn("dbo.Productoes", "UnidadMedida_abreviacion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Productoes", "UnidadMedida_abreviacion", c => c.String());
            AddColumn("dbo.Productoes", "UnidadMedida_nombre", c => c.String());
            DropColumn("dbo.Productoes", "unidadMedida");
        }
    }
}
