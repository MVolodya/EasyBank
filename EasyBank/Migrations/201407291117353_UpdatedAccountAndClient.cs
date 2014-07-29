namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedAccountAndClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "AvailableAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Client", "IsOnlineUser", c => c.Boolean(nullable: false));
            AddColumn("dbo.Client", "IsAlreadyRegistered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Client", "IsAlreadyRegistered");
            DropColumn("dbo.Client", "IsOnlineUser");
            DropColumn("dbo.Account", "AvailableAmount");
        }
    }
}
