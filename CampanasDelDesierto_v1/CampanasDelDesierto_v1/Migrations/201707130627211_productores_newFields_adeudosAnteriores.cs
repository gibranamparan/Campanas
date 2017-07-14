namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productores_newFields_adeudosAnteriores : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productors", "adeudoAnteriorAnticipos", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Productors", "adeudoAnteriorArboles", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Productors", "adeudoAnterior");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Productors", "adeudoAnterior", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Productors", "adeudoAnteriorArboles");
            DropColumn("dbo.Productors", "adeudoAnteriorAnticipos");
        }
    }
}
