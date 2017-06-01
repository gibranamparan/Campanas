namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class campoAgregado_DescripcionConcepto_PrestamoAbonoCapital : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "descripcionConcepto", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "descripcionConcepto");
        }
    }
}
