namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedClientProfile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            AddColumn("dbo.Client", "UserProfile_UserId", c => c.Int());
            CreateIndex("dbo.Client", "UserProfile_UserId");
            AddForeignKey("dbo.Client", "UserProfile_UserId", "dbo.UserProfile", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Client", "UserProfile_UserId", "dbo.UserProfile");
            DropIndex("dbo.Client", new[] { "UserProfile_UserId" });
            DropColumn("dbo.Client", "UserProfile_UserId");
            DropTable("dbo.UserProfile");
        }
    }
}
