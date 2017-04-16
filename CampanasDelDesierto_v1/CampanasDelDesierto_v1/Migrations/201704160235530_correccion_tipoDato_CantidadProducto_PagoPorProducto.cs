namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class correccion_tipoDato_CantidadProducto_PagoPorProducto : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MovimientoFinancieroes", "cantidadProducto", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MovimientoFinancieroes", "cantidadProducto", c => c.Int());
        }
    }
}
