namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datoDesactivadoEnProductor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productors", "desactivado", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Productors", "desactivado");
        }
    }
}
