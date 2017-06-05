namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialcommit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activoes",
                c => new
                    {
                        idActivo = c.Int(nullable: false, identity: true),
                        nombreActivo = c.String(),
                        estadoActivo = c.String(),
                        inventarioID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idActivo)
                .ForeignKey("dbo.Inventarios", t => t.inventarioID, cascadeDelete: true)
                .Index(t => t.inventarioID);
            
            CreateTable(
                "dbo.Inventarios",
                c => new
                    {
                        inventarioID = c.Int(nullable: false, identity: true),
                        nombreInventario = c.String(),
                        modeloInventario = c.String(),
                        costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        cantidad = c.Int(nullable: false),
                        departamentoID = c.Int(),
                    })
                .PrimaryKey(t => t.inventarioID)
                .ForeignKey("dbo.Departamentoes", t => t.departamentoID)
                .Index(t => t.departamentoID);
            
            CreateTable(
                "dbo.Departamentoes",
                c => new
                    {
                        departamentoID = c.Int(nullable: false, identity: true),
                        nombreDepartamento = c.String(),
                        domicilio = c.String(),
                        idSucursal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.departamentoID)
                .ForeignKey("dbo.Sucursals", t => t.idSucursal, cascadeDelete: true)
                .Index(t => t.idSucursal);
            
            CreateTable(
                "dbo.Empleadoes",
                c => new
                    {
                        idEmpleado = c.Int(nullable: false, identity: true),
                        nombre = c.String(),
                        apellidoPaterno = c.String(),
                        apellidoMaterno = c.String(),
                        departamentoID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idEmpleado)
                .ForeignKey("dbo.Departamentoes", t => t.departamentoID, cascadeDelete: true)
                .Index(t => t.departamentoID);
            
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
                "dbo.Sucursals",
                c => new
                    {
                        idSucursal = c.Int(nullable: false, identity: true),
                        nombreSucursal = c.String(),
                    })
                .PrimaryKey(t => t.idSucursal);
            
            CreateTable(
                "dbo.CompraProductoes",
                c => new
                    {
                        compraID = c.Int(nullable: false, identity: true),
                        idProducto = c.Int(),
                        idMovimiento = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.compraID)
                .ForeignKey("dbo.Productoes", t => t.idProducto)
                .ForeignKey("dbo.MovimientoFinancieroes", t => t.idMovimiento, cascadeDelete: true)
                .Index(t => t.idProducto)
                .Index(t => t.idMovimiento);
            
            CreateTable(
                "dbo.Productoes",
                c => new
                    {
                        idProducto = c.Int(nullable: false, identity: true),
                        nombreProducto = c.String(),
                        costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        descripcion = c.String(),
                        UnidadMedida_nombre = c.String(),
                        UnidadMedida_abreviacion = c.String(),
                        isArbolAceituna = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.idProducto);
            
            CreateTable(
                "dbo.MovimientoFinancieroes",
                c => new
                    {
                        idMovimiento = c.Int(nullable: false, identity: true),
                        montoMovimiento = c.Decimal(nullable: false, precision: 18, scale: 2),
                        balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        fechaMovimiento = c.DateTime(nullable: false),
                        idProductor = c.Int(nullable: false),
                        TemporadaDeCosechaID = c.Int(nullable: false),
                        cantidadMaterial = c.Int(),
                        cantidadProducto1 = c.Double(),
                        cantidadProducto2 = c.Double(),
                        cantidadProducto3 = c.Double(),
                        pagoProducto1 = c.Decimal(precision: 18, scale: 2),
                        pagoProducto2 = c.Decimal(precision: 18, scale: 2),
                        pagoProducto3 = c.Decimal(precision: 18, scale: 2),
                        tipoDeDeduccion = c.Int(),
                        cheque = c.String(),
                        cheque1 = c.String(),
                        tipoDeMovimientoDeCapital = c.String(),
                        pagare = c.String(),
                        fechaPagar = c.DateTime(),
                        proveedor = c.String(),
                        concepto = c.String(),
                        descripcionConcepto = c.String(),
                        pozo = c.Int(),
                        precioDelDolar = c.Decimal(precision: 18, scale: 4),
                        divisa = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.idMovimiento)
                .ForeignKey("dbo.Productors", t => t.idProductor, cascadeDelete: true)
                .ForeignKey("dbo.TemporadaDeCosechas", t => t.TemporadaDeCosechaID, cascadeDelete: true)
                .Index(t => t.idProductor)
                .Index(t => t.TemporadaDeCosechaID);
            
            CreateTable(
                "dbo.Productors",
                c => new
                    {
                        idProductor = c.Int(nullable: false, identity: true),
                        numProductor = c.String(),
                        nombreProductor = c.String(),
                        domicilio = c.String(),
                        fechaIntegracion = c.DateTime(nullable: false),
                        RFC = c.String(),
                        zona = c.String(),
                        poblacion = c.String(),
                        nombreCheque = c.String(),
                        nombreRepresentanteLegal = c.String(),
                        telefono = c.String(),
                        adeudoAnterior = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.idProductor);
            
            CreateTable(
                "dbo.TemporadaDeCosechas",
                c => new
                    {
                        TemporadaDeCosechaID = c.Int(nullable: false, identity: true),
                        fechaInicio = c.DateTime(nullable: false),
                        fechaFin = c.DateTime(nullable: false),
                        precioProducto1 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        precioProducto2 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        precioProducto3 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        precioProducto4 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        precioProducto5 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        precioProducto6 = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.TemporadaDeCosechaID);
            
            CreateTable(
                "dbo.RecepcionDeProductoes",
                c => new
                    {
                        recepcionID = c.Int(nullable: false, identity: true),
                        numeroRecibo = c.Int(nullable: false),
                        numProductor = c.String(),
                        nombreProductor = c.String(),
                        cantidadTonsProd1 = c.Double(nullable: false),
                        cantidadTonsProd2 = c.Double(nullable: false),
                        cantidadTonsProd3 = c.Double(nullable: false),
                        fecha = c.DateTime(nullable: false),
                        semana = c.Int(nullable: false),
                        TemporadaDeCosechaID = c.Int(),
                        idProductor = c.Int(),
                        movimientoID = c.Int(),
                    })
                .PrimaryKey(t => t.recepcionID)
                .ForeignKey("dbo.MovimientoFinancieroes", t => t.movimientoID)
                .ForeignKey("dbo.Productors", t => t.idProductor)
                .ForeignKey("dbo.TemporadaDeCosechas", t => t.TemporadaDeCosechaID)
                .Index(t => t.TemporadaDeCosechaID)
                .Index(t => t.idProductor)
                .Index(t => t.movimientoID);
            
            CreateTable(
                "dbo.Conceptos",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nombreConcepto = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Proveedores",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nombreProveedor = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.RecepcionDeProductoes", "TemporadaDeCosechaID", "dbo.TemporadaDeCosechas");
            DropForeignKey("dbo.RecepcionDeProductoes", "idProductor", "dbo.Productors");
            DropForeignKey("dbo.RecepcionDeProductoes", "movimientoID", "dbo.MovimientoFinancieroes");
            DropForeignKey("dbo.MovimientoFinancieroes", "TemporadaDeCosechaID", "dbo.TemporadaDeCosechas");
            DropForeignKey("dbo.MovimientoFinancieroes", "idProductor", "dbo.Productors");
            DropForeignKey("dbo.CompraProductoes", "idMovimiento", "dbo.MovimientoFinancieroes");
            DropForeignKey("dbo.CompraProductoes", "idProducto", "dbo.Productoes");
            DropForeignKey("dbo.Departamentoes", "idSucursal", "dbo.Sucursals");
            DropForeignKey("dbo.Inventarios", "departamentoID", "dbo.Departamentoes");
            DropForeignKey("dbo.PrestamoActivoes", "idEmpleado", "dbo.Empleadoes");
            DropForeignKey("dbo.PrestamoActivoes", "idActivo", "dbo.Activoes");
            DropForeignKey("dbo.Empleadoes", "departamentoID", "dbo.Departamentoes");
            DropForeignKey("dbo.Activoes", "inventarioID", "dbo.Inventarios");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.RecepcionDeProductoes", new[] { "movimientoID" });
            DropIndex("dbo.RecepcionDeProductoes", new[] { "idProductor" });
            DropIndex("dbo.RecepcionDeProductoes", new[] { "TemporadaDeCosechaID" });
            DropIndex("dbo.MovimientoFinancieroes", new[] { "TemporadaDeCosechaID" });
            DropIndex("dbo.MovimientoFinancieroes", new[] { "idProductor" });
            DropIndex("dbo.CompraProductoes", new[] { "idMovimiento" });
            DropIndex("dbo.CompraProductoes", new[] { "idProducto" });
            DropIndex("dbo.PrestamoActivoes", new[] { "idActivo" });
            DropIndex("dbo.PrestamoActivoes", new[] { "idEmpleado" });
            DropIndex("dbo.Empleadoes", new[] { "departamentoID" });
            DropIndex("dbo.Departamentoes", new[] { "idSucursal" });
            DropIndex("dbo.Inventarios", new[] { "departamentoID" });
            DropIndex("dbo.Activoes", new[] { "inventarioID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Proveedores");
            DropTable("dbo.Conceptos");
            DropTable("dbo.RecepcionDeProductoes");
            DropTable("dbo.TemporadaDeCosechas");
            DropTable("dbo.Productors");
            DropTable("dbo.MovimientoFinancieroes");
            DropTable("dbo.Productoes");
            DropTable("dbo.CompraProductoes");
            DropTable("dbo.Sucursals");
            DropTable("dbo.PrestamoActivoes");
            DropTable("dbo.Empleadoes");
            DropTable("dbo.Departamentoes");
            DropTable("dbo.Inventarios");
            DropTable("dbo.Activoes");
        }
    }
}
