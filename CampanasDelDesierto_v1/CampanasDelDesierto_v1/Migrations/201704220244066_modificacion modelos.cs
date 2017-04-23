namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modificacionmodelos : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Departamentoes", "Sucursal_idSucursal", "dbo.Sucursals");
            DropIndex("dbo.Departamentoes", new[] { "Sucursal_idSucursal" });
            RenameColumn(table: "dbo.Departamentoes", name: "Sucursal_idSucursal", newName: "idSucursal");
            AddColumn("dbo.Inventarios", "modeloInventario", c => c.String());
            AddColumn("dbo.Inventarios", "costo", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Activoes", "nombreActivo", c => c.String());
            AlterColumn("dbo.Inventarios", "nombreInventario", c => c.String());
            AlterColumn("dbo.Sucursals", "nombreSucursal", c => c.String());
            AlterColumn("dbo.Sucursals", "domicilioSucursal", c => c.String());
            AlterColumn("dbo.Departamentoes", "nombreDepartamento", c => c.String());
            AlterColumn("dbo.Departamentoes", "idSucursal", c => c.Int(nullable: false));
            AlterColumn("dbo.Empleadoes", "nombre", c => c.String());
            AlterColumn("dbo.Empleadoes", "apellidoPaterno", c => c.String());
            AlterColumn("dbo.Empleadoes", "apellidoMaterno", c => c.String());
            CreateIndex("dbo.Departamentoes", "idSucursal");
            AddForeignKey("dbo.Departamentoes", "idSucursal", "dbo.Sucursals", "idSucursal", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Departamentoes", "idSucursal", "dbo.Sucursals");
            DropIndex("dbo.Departamentoes", new[] { "idSucursal" });
            AlterColumn("dbo.Empleadoes", "apellidoMaterno", c => c.String(nullable: false));
            AlterColumn("dbo.Empleadoes", "apellidoPaterno", c => c.String(nullable: false));
            AlterColumn("dbo.Empleadoes", "nombre", c => c.String(nullable: false));
            AlterColumn("dbo.Departamentoes", "idSucursal", c => c.Int());
            AlterColumn("dbo.Departamentoes", "nombreDepartamento", c => c.String(nullable: false));
            AlterColumn("dbo.Sucursals", "domicilioSucursal", c => c.String(nullable: false));
            AlterColumn("dbo.Sucursals", "nombreSucursal", c => c.String(nullable: false));
            AlterColumn("dbo.Inventarios", "nombreInventario", c => c.String(nullable: false));
            AlterColumn("dbo.Activoes", "nombreActivo", c => c.String(nullable: false));
            DropColumn("dbo.Inventarios", "costo");
            DropColumn("dbo.Inventarios", "modeloInventario");
            RenameColumn(table: "dbo.Departamentoes", name: "idSucursal", newName: "Sucursal_idSucursal");
            CreateIndex("dbo.Departamentoes", "Sucursal_idSucursal");
            AddForeignKey("dbo.Departamentoes", "Sucursal_idSucursal", "dbo.Sucursals", "idSucursal");
        }
    }
}
