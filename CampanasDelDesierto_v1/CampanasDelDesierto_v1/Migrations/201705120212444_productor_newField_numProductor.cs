namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productor_newField_numProductor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productors", "numProductor", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Productors", "numProductor");
        }
    }
}
