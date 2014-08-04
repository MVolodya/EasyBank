namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedIncorrectPasswordTrialsToClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Client", "IncorrectPasswordTrials", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Client", "IncorrectPasswordTrials");
        }
    }
}
