namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambioenmodelodeactivoempleadosucursalyprestamoactivo : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Activoes", "idPrestamo");
            DropColumn("dbo.Empleadoes", "idPrestamo");
            DropColumn("dbo.Sucursals", "idEmpleado");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sucursals", "idEmpleado", c => c.Int(nullable: false));
            AddColumn("dbo.Empleadoes", "idPrestamo", c => c.Int(nullable: false));
            AddColumn("dbo.Activoes", "idPrestamo", c => c.Int(nullable: false));
        }
    }
}
