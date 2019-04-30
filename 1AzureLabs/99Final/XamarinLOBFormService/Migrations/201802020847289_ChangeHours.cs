namespace XamarinLOBFormService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeHours : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeaveAppForms", "Hours", c => c.Double(nullable: false));
            AddColumn("dbo.WorkingLogs", "Hours", c => c.Double(nullable: false));
            DropColumn("dbo.LeaveAppForms", "TotalHours");
            DropColumn("dbo.WorkingLogs", "TotalHours");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkingLogs", "TotalHours", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.LeaveAppForms", "TotalHours", c => c.Time(nullable: false, precision: 7));
            DropColumn("dbo.WorkingLogs", "Hours");
            DropColumn("dbo.LeaveAppForms", "Hours");
        }
    }
}
