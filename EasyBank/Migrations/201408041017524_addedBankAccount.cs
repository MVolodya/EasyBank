namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedBankAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankAccount",
                c => new
                    {
                        BankAccountId = c.Int(nullable: false, identity: true),
                        CurrencyName = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BankAccountId);
            
            AddColumn("dbo.Client", "InitialPassword", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Client", "InitialPassword");
            DropTable("dbo.BankAccount");
        }
    }
}
