namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOperator : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Operator",
                c => new
                    {
                        OperatorID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Surname = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        UserProfile_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.OperatorID)
                .ForeignKey("dbo.UserProfile", t => t.UserProfile_UserId)
                .Index(t => t.UserProfile_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Operator", "UserProfile_UserId", "dbo.UserProfile");
            DropIndex("dbo.Operator", new[] { "UserProfile_UserId" });
            DropTable("dbo.Operator");
        }
    }
}
