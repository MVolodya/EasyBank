namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DepCredModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DepositCreditModel",
                c => new
                    {
                        DepositCreditModelID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Duration = c.Int(nullable: false),
                        InterestRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EarlyTermination = c.Boolean(nullable: false),
                        AccountTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DepositCreditModelID)
                .ForeignKey("dbo.AccountType", t => t.AccountTypeId, cascadeDelete: true)
                .Index(t => t.AccountTypeId);
            
            AddColumn("dbo.Account", "Interest", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Account", "DepositCreditModelID", c => c.Int());
            CreateIndex("dbo.Account", "DepositCreditModelID");
            AddForeignKey("dbo.Account", "DepositCreditModelID", "dbo.DepositCreditModel", "DepositCreditModelID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Account", "DepositCreditModelID", "dbo.DepositCreditModel");
            DropForeignKey("dbo.DepositCreditModel", "AccountTypeId", "dbo.AccountType");
            DropIndex("dbo.DepositCreditModel", new[] { "AccountTypeId" });
            DropIndex("dbo.Account", new[] { "DepositCreditModelID" });
            DropColumn("dbo.Account", "DepositCreditModelID");
            DropColumn("dbo.Account", "Interest");
            DropTable("dbo.DepositCreditModel");
        }
    }
}
