namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PurchaseSellRateFieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Currency", "PurchaseRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Currency", "SaleRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.ErrorReport", "AccountId", c => c.Int(nullable: false));
            CreateIndex("dbo.ErrorReport", "AccountId");
            AddForeignKey("dbo.ErrorReport", "AccountId", "dbo.Account", "AccountId", cascadeDelete: true);
            DropColumn("dbo.Account", "Interest");
            DropColumn("dbo.Currency", "ExchangeRate");
            DropColumn("dbo.ErrorReport", "AccountNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ErrorReport", "AccountNumber", c => c.String());
            AddColumn("dbo.Currency", "ExchangeRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Account", "Interest", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.ErrorReport", "AccountId", "dbo.Account");
            DropIndex("dbo.ErrorReport", new[] { "AccountId" });
            DropColumn("dbo.ErrorReport", "AccountId");
            DropColumn("dbo.Currency", "SaleRate");
            DropColumn("dbo.Currency", "PurchaseRate");
        }
    }
}
