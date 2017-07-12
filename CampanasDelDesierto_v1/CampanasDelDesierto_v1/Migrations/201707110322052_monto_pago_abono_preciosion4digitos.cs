namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class monto_pago_abono_preciosion4digitos : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Prestamo_Abono", "monto", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Prestamo_Abono", "monto", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
