namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productor_ModificadosCamposPrecios : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Productors", "precioCosecha");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Productors", "precioCosecha", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
