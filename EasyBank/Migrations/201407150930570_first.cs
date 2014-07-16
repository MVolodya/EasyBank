namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Account",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        AccountNumber = c.String(),
                        Currency = c.Int(nullable: false),
                        ExpirationDate = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Client", t => t.ClientID, cascadeDelete: true)
                .Index(t => t.ClientID);
            
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Surname = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false),
                        RegistrationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.CurrencyOperation",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FromAccountID = c.Int(nullable: false),
                        ToAccountID = c.Int(nullable: false),
                        TransferAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Account", t => t.ToAccountID)
                .ForeignKey("dbo.Account", t => t.FromAccountID)
                .Index(t => t.FromAccountID)
                .Index(t => t.ToAccountID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CurrencyOperation", "FromAccountID", "dbo.Account");
            DropForeignKey("dbo.CurrencyOperation", "ToAccountID", "dbo.Account");
            DropForeignKey("dbo.Account", "ClientID", "dbo.Client");
            DropIndex("dbo.CurrencyOperation", new[] { "ToAccountID" });
            DropIndex("dbo.CurrencyOperation", new[] { "FromAccountID" });
            DropIndex("dbo.Account", new[] { "ClientID" });
            DropTable("dbo.CurrencyOperation");
            DropTable("dbo.Client");
            DropTable("dbo.Account");
        }
    }
}
