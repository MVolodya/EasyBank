namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOperationsHistory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CurrencyOperation", "ToAccountId", "dbo.Account");
            DropForeignKey("dbo.CurrencyOperation", "FromAccountId", "dbo.Account");
            DropIndex("dbo.CurrencyOperation", new[] { "FromAccountId" });
            DropIndex("dbo.CurrencyOperation", new[] { "ToAccountId" });
            CreateTable(
                "dbo.Operation",
                c => new
                    {
                        OperationId = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        FromAccountId = c.Int(nullable: false),
                        ToAccountId = c.Int(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.OperationId)
                .ForeignKey("dbo.Account", t => t.ToAccountId)
                .ForeignKey("dbo.Account", t => t.FromAccountId)
                .Index(t => t.FromAccountId)
                .Index(t => t.ToAccountId);
            
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
            
            DropForeignKey("dbo.Operation", "FromAccountId", "dbo.Account");
            DropForeignKey("dbo.Operation", "ToAccountId", "dbo.Account");
            DropIndex("dbo.Operation", new[] { "ToAccountId" });
            DropIndex("dbo.Operation", new[] { "FromAccountId" });
            DropIndex("dbo.Account", "UniqueAccNumb");
            AlterColumn("dbo.Account", "AccountNumber", c => c.String(nullable: false, maxLength: 8));
            DropTable("dbo.Operation");
            CreateIndex("dbo.CurrencyOperation", "ToAccountId");
            CreateIndex("dbo.CurrencyOperation", "FromAccountId");
            AddForeignKey("dbo.CurrencyOperation", "FromAccountId", "dbo.Account", "AccountId");
            AddForeignKey("dbo.CurrencyOperation", "ToAccountId", "dbo.Account", "AccountId");
        }
    }
}
