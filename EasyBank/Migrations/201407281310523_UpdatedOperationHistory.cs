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
            RenameColumn(table: "dbo.Operation", name: "FromAccountId", newName: "AccountId");
            AddColumn("dbo.Operation", "OperatorId", c => c.Int());
            AddColumn("dbo.Operator", "RegistrationDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Operation", "AccountId", c => c.Int());
            AlterColumn("dbo.Operation", "ToAccountId", c => c.Int());
            CreateIndex("dbo.Operation", "OperatorId");
            CreateIndex("dbo.Operation", "AccountId");
            CreateIndex("dbo.Operation", "ToAccountId");
            AddForeignKey("dbo.Operation", "OperatorId", "dbo.Operator", "OperatorID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Operation", "OperatorId", "dbo.Operator");
            DropIndex("dbo.Operation", new[] { "ToAccountId" });
            DropIndex("dbo.Operation", new[] { "AccountId" });
            DropIndex("dbo.Operation", new[] { "OperatorId" });
            AlterColumn("dbo.Operation", "ToAccountId", c => c.Int(nullable: false));
            AlterColumn("dbo.Operation", "AccountId", c => c.Int(nullable: false));
            DropColumn("dbo.Operator", "RegistrationDate");
            DropColumn("dbo.Operation", "OperatorId");
            RenameColumn(table: "dbo.Operation", name: "AccountId", newName: "FromAccountId");
            CreateIndex("dbo.Operation", "ToAccountId");
            CreateIndex("dbo.Operation", "FromAccountId");
        }
    }
}
