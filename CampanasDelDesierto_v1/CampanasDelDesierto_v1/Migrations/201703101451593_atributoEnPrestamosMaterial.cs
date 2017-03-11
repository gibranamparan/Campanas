namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class atributoEnPrestamosMaterial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "cantidadMaterial", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "cantidadMaterial");
        }
    }
}
