namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nueva_entidad_AdeudoInicial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "balanceAdeudado", c => c.Int());
            AddColumn("dbo.MovimientoFinancieroes", "interesInicial", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Productors", "adeudoAnteriorAnticipos");
            DropColumn("dbo.Productors", "adeudoAnteriorArboles");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Productors", "adeudoAnteriorArboles", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Productors", "adeudoAnteriorAnticipos", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.MovimientoFinancieroes", "interesInicial");
            DropColumn("dbo.MovimientoFinancieroes", "balanceAdeudado");
        }
    }
}
