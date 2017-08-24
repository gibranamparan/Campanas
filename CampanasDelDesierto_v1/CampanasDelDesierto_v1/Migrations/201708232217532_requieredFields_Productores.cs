namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requieredFields_Productores : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Productors", "numProductor", c => c.String(nullable: false));
            AlterColumn("dbo.Productors", "nombreProductor", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Productors", "nombreProductor", c => c.String());
            AlterColumn("dbo.Productors", "numProductor", c => c.String());
        }
    }
}
