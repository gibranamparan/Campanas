namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreacionTablasConCamposNuevos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activoes",
                c => new
                    {
                        idActivos = c.Int(nullable: false, identity: true),
                        nombreActivo = c.String(),
                        costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        estado = c.String(),
                        fecha = c.DateTime(nullable: false),
                        concepto = c.String(),
                        pagare = c.String(),
                        ordenDeCompra = c.String(),
                    })
                .PrimaryKey(t => t.idActivos);
            
            CreateTable(
                "dbo.MovimientoFinancieroes",
                c => new
                    {
                        idMovimiento = c.Int(nullable: false, identity: true),
                        montoMovimiento = c.Int(nullable: false),
                        fechaMovimiento = c.DateTime(nullable: false),
                        idProductor = c.Int(nullable: false),
                        cantidadMaterial = c.Int(),
                        idActivos = c.Int(),
                        cantidadProducto = c.Double(),
                        numeroSemana = c.Int(),
                        cheque = c.String(),
                        MyProperty = c.Int(),
                        tipoProducto = c.String(),
                        garantiaLimpieza = c.Int(),
                        fechaDePrestamo = c.DateTime(),
                        cheque1 = c.String(),
                        concepto = c.String(),
                        cargo = c.Double(),
                        pagare = c.String(),
                        fechaPagar = c.DateTime(),
                        proveedor = c.String(),
                        nota = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.idMovimiento)
                .ForeignKey("dbo.Activoes", t => t.idActivos, cascadeDelete: true)
                .ForeignKey("dbo.Productors", t => t.idProductor, cascadeDelete: true)
                .Index(t => t.idProductor)
                .Index(t => t.idActivos);
            
            CreateTable(
                "dbo.Productors",
                c => new
                    {
                        idProductor = c.Int(nullable: false, identity: true),
                        nombreProductor = c.String(),
                        domicilio = c.String(),
                        fechaIntegracion = c.DateTime(nullable: false),
                        RFC = c.String(),
                        zona = c.String(),
                        nombreCheque = c.String(),
                        adeudoAnterior = c.Int(),
                        precioCosecha = c.Int(),
                    })
                .PrimaryKey(t => t.idProductor);
            
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
            DropForeignKey("dbo.MovimientoFinancieroes", "idProductor", "dbo.Productors");
            DropForeignKey("dbo.MovimientoFinancieroes", "idActivos", "dbo.Activoes");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.MovimientoFinancieroes", new[] { "idActivos" });
            DropIndex("dbo.MovimientoFinancieroes", new[] { "idProductor" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Productors");
            DropTable("dbo.MovimientoFinancieroes");
            DropTable("dbo.Activoes");
        }
    }
}
