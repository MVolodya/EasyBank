namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedOperationHistory : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Operation", new[] { "FromAccountId" });
            DropIndex("dbo.Operation", new[] { "ToAccountId" });
            AddColumn("dbo.Operation", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Operation", "OperatorId", c => c.Int());
            AddColumn("dbo.Operator", "RegistrationDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Operation", "FromAccountId", c => c.Int());
            AlterColumn("dbo.Operation", "ToAccountId", c => c.Int());
            CreateIndex("dbo.Operation", "OperatorId");
            CreateIndex("dbo.Operation", "FromAccountId");
            CreateIndex("dbo.Operation", "ToAccountId");
            AddForeignKey("dbo.Operation", "OperatorId", "dbo.Operator", "OperatorID");
            DropColumn("dbo.Operation", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Operation", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.Operation", "OperatorId", "dbo.Operator");
            DropIndex("dbo.Operation", new[] { "ToAccountId" });
            DropIndex("dbo.Operation", new[] { "FromAccountId" });
            DropIndex("dbo.Operation", new[] { "OperatorId" });
            AlterColumn("dbo.Operation", "ToAccountId", c => c.Int(nullable: false));
            AlterColumn("dbo.Operation", "FromAccountId", c => c.Int(nullable: false));
            DropColumn("dbo.Operator", "RegistrationDate");
            DropColumn("dbo.Operation", "OperatorId");
            DropColumn("dbo.Operation", "Type");
            CreateIndex("dbo.Operation", "ToAccountId");
            CreateIndex("dbo.Operation", "FromAccountId");
        }
    }
}
