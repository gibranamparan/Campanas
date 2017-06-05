namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cantidad_ComprasProductos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompraProductoes", "cantidadMaterial", c => c.Int(nullable: false));
            DropColumn("dbo.MovimientoFinancieroes", "cantidadMaterial");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "cantidadMaterial", c => c.Int());
            DropColumn("dbo.CompraProductoes", "cantidadMaterial");
        }
    }
}
