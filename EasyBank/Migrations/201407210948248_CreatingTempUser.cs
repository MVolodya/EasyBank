namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatingTempUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Image",
                c => new
                    {
                        ImageID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ContentType = c.String(),
                        ImageContent = c.Binary(),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ImageID)
                .ForeignKey("dbo.Client", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Image", "ClientId", "dbo.Client");
            DropIndex("dbo.Image", new[] { "ClientId" });
            DropTable("dbo.Image");
        }
    }
}
