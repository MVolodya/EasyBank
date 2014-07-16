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
                        AccountId = c.Int(nullable: false, identity: true),
                        AccountNumber = c.String(nullable: false),
                        ExpirationDate = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClientId = c.Int(nullable: false),
                        TypeId = c.Int(nullable: false),
                        CurrencyId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountId)
                .ForeignKey("dbo.AccountStatus", t => t.StatusId, cascadeDelete: true)
                .ForeignKey("dbo.AccountType", t => t.TypeId, cascadeDelete: true)
                .ForeignKey("dbo.Client", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Currency", t => t.CurrencyId, cascadeDelete: true)
                .Index(t => t.ClientId)
                .Index(t => t.TypeId)
                .Index(t => t.CurrencyId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.AccountStatus",
                c => new
                    {
                        StatusId = c.Int(nullable: false, identity: true),
                        StatusName = c.String(),
                    })
                .PrimaryKey(t => t.StatusId);
            
            CreateTable(
                "dbo.AccountType",
                c => new
                    {
                        TypeId = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                    })
                .PrimaryKey(t => t.TypeId);
            
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        ClientId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Surname = c.String(nullable: false, maxLength: 50),
                        PIdNumber = c.String(nullable: false, maxLength: 10),
                        BirthDate = c.DateTime(nullable: false),
                        Email = c.String(nullable: false),
                        RegistrationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ClientId);
            
            CreateTable(
                "dbo.Currency",
                c => new
                    {
                        CurrencyId = c.Int(nullable: false, identity: true),
                        CurrencyName = c.String(),
                    })
                .PrimaryKey(t => t.CurrencyId);
            
            CreateTable(
                "dbo.CurrencyOperation",
                c => new
                    {
                        CurrencyOperationId = c.Int(nullable: false, identity: true),
                        FromAccountId = c.Int(nullable: false),
                        ToAccountId = c.Int(nullable: false),
                        TransferAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CurrencyOperationId)
                .ForeignKey("dbo.Account", t => t.ToAccountId)
                .ForeignKey("dbo.Account", t => t.FromAccountId)
                .Index(t => t.FromAccountId)
                .Index(t => t.ToAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CurrencyOperation", "FromAccountId", "dbo.Account");
            DropForeignKey("dbo.CurrencyOperation", "ToAccountId", "dbo.Account");
            DropForeignKey("dbo.Account", "CurrencyId", "dbo.Currency");
            DropForeignKey("dbo.Account", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Account", "TypeId", "dbo.AccountType");
            DropForeignKey("dbo.Account", "StatusId", "dbo.AccountStatus");
            DropIndex("dbo.CurrencyOperation", new[] { "ToAccountId" });
            DropIndex("dbo.CurrencyOperation", new[] { "FromAccountId" });
            DropIndex("dbo.Account", new[] { "StatusId" });
            DropIndex("dbo.Account", new[] { "CurrencyId" });
            DropIndex("dbo.Account", new[] { "TypeId" });
            DropIndex("dbo.Account", new[] { "ClientId" });
            DropTable("dbo.CurrencyOperation");
            DropTable("dbo.Currency");
            DropTable("dbo.Client");
            DropTable("dbo.AccountType");
            DropTable("dbo.AccountStatus");
            DropTable("dbo.Account");
        }
    }
}
