namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedFiedls_poblacion_telefono : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productors", "poblacion", c => c.String());
            AddColumn("dbo.Productors", "telefono", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Productors", "telefono");
            DropColumn("dbo.Productors", "poblacion");
        }
    }
}
