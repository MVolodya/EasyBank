namespace EasyBank.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedErrorReportAndUpdatedOperatorAndCurrencyModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ErrorReport",
                c => new
                    {
                        ErrorReportId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        AccountNumber = c.String(),
                        Solved = c.Boolean(nullable: false),
                        ReportDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ErrorReportId);
            
            AddColumn("dbo.Currency", "ExchangeRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Operator", "DepartmentInfo", c => c.String());
            AddColumn("dbo.Operator", "PId", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.Operator", "Name", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Operator", "Surname", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Operator", "Email", c => c.String(nullable: false));
            CreateIndex("dbo.Operator", "PId", unique: true, name: "UniqueOperatorPId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Operator", "UniqueOperatorPId");
            AlterColumn("dbo.Operator", "Email", c => c.String());
            AlterColumn("dbo.Operator", "Surname", c => c.String());
            AlterColumn("dbo.Operator", "Name", c => c.String());
            DropColumn("dbo.Operator", "PId");
            DropColumn("dbo.Operator", "DepartmentInfo");
            DropColumn("dbo.Currency", "ExchangeRate");
            DropTable("dbo.ErrorReport");
        }
    }
}
