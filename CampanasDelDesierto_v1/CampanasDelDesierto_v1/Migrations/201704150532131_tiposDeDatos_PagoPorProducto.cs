namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tiposDeDatos_PagoPorProducto : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MovimientoFinancieroes", "cantidadProducto", c => c.Int());
            AlterColumn("dbo.MovimientoFinancieroes", "abonoAnticipo", c => c.Double());
            AlterColumn("dbo.MovimientoFinancieroes", "garantiaLimpieza", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MovimientoFinancieroes", "garantiaLimpieza", c => c.Int());
            AlterColumn("dbo.MovimientoFinancieroes", "abonoAnticipo", c => c.Int());
            AlterColumn("dbo.MovimientoFinancieroes", "cantidadProducto", c => c.Double());
        }
    }
}
