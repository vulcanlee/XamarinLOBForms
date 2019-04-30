namespace XamarinLOBFormService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changetostring : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LeaveCategories", "LeaveCategoryName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LeaveCategories", "LeaveCategoryName", c => c.DateTime(nullable: false));
        }
    }
}
