namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ErrorMessagesAdded : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AccountStatus", "StatusName", c => c.String(nullable: false));
            AlterColumn("dbo.AccountType", "TypeName", c => c.String(nullable: false));
            AlterColumn("dbo.Client", "Name", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Client", "Surname", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Currency", "CurrencyName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Currency", "CurrencyName", c => c.String());
            AlterColumn("dbo.Client", "Surname", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Client", "Name", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.AccountType", "TypeName", c => c.String());
            AlterColumn("dbo.AccountStatus", "StatusName", c => c.String());
        }
    }
}
