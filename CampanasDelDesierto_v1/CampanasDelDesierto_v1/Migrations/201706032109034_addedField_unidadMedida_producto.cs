namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedField_unidadMedida_producto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productoes", "UnidadMedida_nombre", c => c.String());
            AddColumn("dbo.Productoes", "UnidadMedida_abreviacion", c => c.String());
            DropColumn("dbo.Productoes", "estado");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Productoes", "estado", c => c.String());
            DropColumn("dbo.Productoes", "UnidadMedida_abreviacion");
            DropColumn("dbo.Productoes", "UnidadMedida_nombre");
        }
    }
}
