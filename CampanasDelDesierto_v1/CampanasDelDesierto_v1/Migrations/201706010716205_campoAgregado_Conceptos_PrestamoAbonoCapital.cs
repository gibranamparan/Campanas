namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class campoAgregado_Conceptos_PrestamoAbonoCapital : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MovimientoFinancieroes", "concepto", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MovimientoFinancieroes", "concepto");
        }
    }
}
