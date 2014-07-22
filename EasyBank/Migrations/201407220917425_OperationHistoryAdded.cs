namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OperationHistoryAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DepositOperation",
                c => new
                    {
                        OperationId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OperationId)
                .ForeignKey("dbo.Account", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.TransferOperation",
                c => new
                    {
                        OperationId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccountId = c.Int(nullable: false),
                        ToAccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OperationId)
                .ForeignKey("dbo.Account", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("dbo.Account", t => t.ToAccountId, cascadeDelete: true)
                .Index(t => t.AccountId)
                .Index(t => t.ToAccountId);
            
            CreateTable(
                "dbo.WithdrawOperation",
                c => new
                    {
                        OperationId = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        AccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OperationId)
                .ForeignKey("dbo.Account", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WithdrawOperation", "AccountId", "dbo.Account");
            DropForeignKey("dbo.TransferOperation", "ToAccountId", "dbo.Account");
            DropForeignKey("dbo.TransferOperation", "AccountId", "dbo.Account");
            DropForeignKey("dbo.DepositOperation", "AccountId", "dbo.Account");
            DropIndex("dbo.WithdrawOperation", new[] { "AccountId" });
            DropIndex("dbo.TransferOperation", new[] { "ToAccountId" });
            DropIndex("dbo.TransferOperation", new[] { "AccountId" });
            DropIndex("dbo.DepositOperation", new[] { "AccountId" });
            DropTable("dbo.WithdrawOperation");
            DropTable("dbo.TransferOperation");
            DropTable("dbo.DepositOperation");
        }
    }
}
