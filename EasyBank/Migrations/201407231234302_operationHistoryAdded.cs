namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class operationHistoryAdded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CurrencyOperation", "ToAccountId", "dbo.Account");
            DropForeignKey("dbo.CurrencyOperation", "FromAccountId", "dbo.Account");
            DropIndex("dbo.CurrencyOperation", new[] { "FromAccountId" });
            DropIndex("dbo.CurrencyOperation", new[] { "ToAccountId" });
            CreateTable(
                "dbo.BinaryOperation",
                c => new
                    {
                        OperationId = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        AccountId = c.Int(nullable: false),
                        ToAccountId = c.Int(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.OperationId)
                .ForeignKey("dbo.Account", t => t.ToAccountId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .Index(t => t.AccountId)
                .Index(t => t.ToAccountId);
            
            CreateTable(
                "dbo.UnaryOperation",
                c => new
                    {
                        OperationId = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        AccountId = c.Int(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.OperationId)
                .ForeignKey("dbo.Account", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
            AlterColumn("dbo.Account", "AccountNumber", c => c.String(maxLength: 10));
            CreateIndex("dbo.Account", "AccountNumber", unique: true, name: "UniqueAccNumb");
            DropTable("dbo.CurrencyOperation");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CurrencyOperation",
                c => new
                    {
                        CurrencyOperationId = c.Int(nullable: false, identity: true),
                        FromAccountId = c.Int(nullable: false),
                        ToAccountId = c.Int(nullable: false),
                        TransferAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CurrencyOperationId);
            
            DropForeignKey("dbo.UnaryOperation", "AccountId", "dbo.Account");
            DropForeignKey("dbo.BinaryOperation", "AccountId", "dbo.Account");
            DropForeignKey("dbo.BinaryOperation", "ToAccountId", "dbo.Account");
            DropIndex("dbo.UnaryOperation", new[] { "AccountId" });
            DropIndex("dbo.BinaryOperation", new[] { "ToAccountId" });
            DropIndex("dbo.BinaryOperation", new[] { "AccountId" });
            DropIndex("dbo.Account", "UniqueAccNumb");
            AlterColumn("dbo.Account", "AccountNumber", c => c.String(nullable: false, maxLength: 8));
            DropTable("dbo.UnaryOperation");
            DropTable("dbo.BinaryOperation");
            CreateIndex("dbo.CurrencyOperation", "ToAccountId");
            CreateIndex("dbo.CurrencyOperation", "FromAccountId");
            AddForeignKey("dbo.CurrencyOperation", "FromAccountId", "dbo.Account", "AccountId");
            AddForeignKey("dbo.CurrencyOperation", "ToAccountId", "dbo.Account", "AccountId");
        }
    }
}
