namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pagos_prestamos_addedField_isINteres : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prestamo_Abono", "pagoAInteres", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Prestamo_Abono", "pagoAInteres");
        }
    }
}
