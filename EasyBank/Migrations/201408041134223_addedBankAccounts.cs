namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedBankAccounts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankAccount",
                c => new
                    {
                        BankAccountId = c.Int(nullable: false, identity: true),
                        CurrencyName = c.String(),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BankAccountId);
            
            AddColumn("dbo.Client", "InitialPassword", c => c.String());
            AlterColumn("dbo.Currency", "CurrencyName", c => c.String(nullable: false, maxLength: 10));
            CreateIndex("dbo.Currency", "CurrencyName", unique: true, name: "UniqueCurrencyName");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Currency", "UniqueCurrencyName");
            AlterColumn("dbo.Currency", "CurrencyName", c => c.String(nullable: false));
            DropColumn("dbo.Client", "InitialPassword");
            DropTable("dbo.BankAccount");
        }
    }
}
