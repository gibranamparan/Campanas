namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nuevodiseñobasededatosempleadossucursalactivos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activoes",
                c => new
                    {
                        idActivo = c.Int(nullable: false, identity: true),
                        nombreActivo = c.String(nullable: false),
                        estadoActivo = c.String(),
                        idPrestamo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idActivo);
            
            CreateTable(
                "dbo.PrestamoActivoes",
                c => new
                    {
                        idPrestamoActivo = c.Int(nullable: false, identity: true),
                        fechaPrestamoActivo = c.DateTime(nullable: false),
                        fechaEntregaActivo = c.DateTime(nullable: false),
                        fechaDevolucion = c.DateTime(),
                        idEmpleado = c.Int(nullable: false),
                        idActivo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idPrestamoActivo)
                .ForeignKey("dbo.Activoes", t => t.idActivo, cascadeDelete: true)
                .ForeignKey("dbo.Empleadoes", t => t.idEmpleado, cascadeDelete: true)
                .Index(t => t.idEmpleado)
                .Index(t => t.idActivo);
            
            CreateTable(
                "dbo.Empleadoes",
                c => new
                    {
                        idEmpleado = c.Int(nullable: false, identity: true),
                        nombre = c.String(nullable: false),
                        apellidoPaterno = c.String(nullable: false),
                        apellidoMaterno = c.String(nullable: false),
                        idPrestamo = c.Int(nullable: false),
                        idSucursal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idEmpleado)
                .ForeignKey("dbo.Sucursals", t => t.idSucursal, cascadeDelete: true)
                .Index(t => t.idSucursal);
            
            CreateTable(
                "dbo.Sucursals",
                c => new
                    {
                        idSucursal = c.Int(nullable: false, identity: true),
                        nombreSucursal = c.String(nullable: false),
                        domicilioSucursal = c.String(nullable: false),
                        idEmpleado = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idSucursal);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Empleadoes", "idSucursal", "dbo.Sucursals");
            DropForeignKey("dbo.PrestamoActivoes", "idEmpleado", "dbo.Empleadoes");
            DropForeignKey("dbo.PrestamoActivoes", "idActivo", "dbo.Activoes");
            DropIndex("dbo.Empleadoes", new[] { "idSucursal" });
            DropIndex("dbo.PrestamoActivoes", new[] { "idActivo" });
            DropIndex("dbo.PrestamoActivoes", new[] { "idEmpleado" });
            DropTable("dbo.Sucursals");
            DropTable("dbo.Empleadoes");
            DropTable("dbo.PrestamoActivoes");
            DropTable("dbo.Activoes");
        }
    }
}
