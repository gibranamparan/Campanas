namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proveedores : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Proveedores",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nombreProveedor = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
           
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Proveedors",
                c => new
                    {
                        idProveedor = c.Int(nullable: false, identity: true),
                        nombreProveedor = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.idProveedor);
            
            DropTable("dbo.Proveedores");
        }
    }
}
