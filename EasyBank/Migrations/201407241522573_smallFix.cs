namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class smallFix : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Client", "PIdNumber", unique: true, name: "UniquePId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Client", "UniquePId");
        }
    }
}
