namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedImageModel : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Image", newName: "ClientsImage");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ClientsImage", newName: "Image");
        }
    }
}
