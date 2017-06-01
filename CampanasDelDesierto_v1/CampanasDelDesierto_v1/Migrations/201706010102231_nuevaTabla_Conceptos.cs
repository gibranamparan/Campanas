namespace CampanasDelDesierto_v1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nuevaTabla_Conceptos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Conceptos",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nombreConcepto = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Conceptos");
        }
    }
}
