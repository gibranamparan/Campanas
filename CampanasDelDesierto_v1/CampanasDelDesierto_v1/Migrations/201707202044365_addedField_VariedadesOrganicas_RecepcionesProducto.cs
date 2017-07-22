namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedField_VariedadesOrganicas_RecepcionesProducto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RecepcionDeProductoes", "cantidadTonsProd5", c => c.Double(nullable: false, defaultValue:0));
            AddColumn("dbo.RecepcionDeProductoes", "cantidadTonsProd6", c => c.Double(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RecepcionDeProductoes", "cantidadTonsProd6");
            DropColumn("dbo.RecepcionDeProductoes", "cantidadTonsProd5");
        }
    }
}
