namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedInterestCalculation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "Interest", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Account", "OpenDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Account", "LastInterestAdded", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "LastInterestAdded");
            DropColumn("dbo.Account", "OpenDate");
            DropColumn("dbo.Account", "Interest");
        }
    }
}
