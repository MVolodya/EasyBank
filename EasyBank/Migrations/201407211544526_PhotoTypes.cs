namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhotoTypes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Image", "PhotoType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Image", "PhotoType");
        }
    }
}
