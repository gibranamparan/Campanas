namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedField_nombreLegal_Productor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productors", "nombreRepresentanteLegal", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Productors", "nombreRepresentanteLegal");
        }
    }
}
