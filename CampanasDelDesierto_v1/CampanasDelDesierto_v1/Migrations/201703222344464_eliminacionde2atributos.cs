namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eliminacionde2atributos : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MovimientoFinancieroes", "fechaDePrestamo");
            DropColumn("dbo.MovimientoFinancieroes", "cargo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MovimientoFinancieroes", "cargo", c => c.Double());
            AddColumn("dbo.MovimientoFinancieroes", "fechaDePrestamo", c => c.DateTime());
        }
    }
}
