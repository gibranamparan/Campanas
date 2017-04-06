namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productor_preciosADecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Productors", "adeudoAnterior", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Productors", "precioCosecha", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Productors", "precioCosecha", c => c.Int());
            AlterColumn("dbo.Productors", "adeudoAnterior", c => c.Int());
        }
    }
}
