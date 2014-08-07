namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class validationsAdded : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DepositCreditModel", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DepositCreditModel", "Name", c => c.String());
        }
    }
}
