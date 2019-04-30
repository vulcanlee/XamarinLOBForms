namespace XamarinLOBFormService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LeaveAppForms",
                c => new
                    {
                        LeaveAppFormId = c.Int(nullable: false, identity: true),
                        FormDate = c.DateTime(nullable: false),
                        Category = c.String(),
                        BeginDate = c.DateTime(nullable: false),
                        CompleteDate = c.DateTime(nullable: false),
                        TotalHours = c.Time(nullable: false, precision: 7),
                        AgentName = c.String(),
                        LeaveCause = c.String(),
                        FormsStatus = c.String(),
                        ApproveResult = c.String(),
                        Owner_MyUserId = c.Int(),
                    })
                .PrimaryKey(t => t.LeaveAppFormId)
                .ForeignKey("dbo.MyUsers", t => t.Owner_MyUserId)
                .Index(t => t.Owner_MyUserId);
            
            CreateTable(
                "dbo.MyUsers",
                c => new
                    {
                        MyUserId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(),
                        Name = c.String(),
                        EmployeeID = c.String(),
                        Password = c.String(),
                        IsManager = c.Boolean(nullable: false),
                        CreatedAt = c.DateTimeOffset(precision: 7),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Manager_MyUserId = c.Int(),
                    })
                .PrimaryKey(t => t.MyUserId)
                .ForeignKey("dbo.MyUsers", t => t.Manager_MyUserId)
                .Index(t => t.Manager_MyUserId);
            
            CreateTable(
                "dbo.LeaveCategories",
                c => new
                    {
                        LeaveCategoryId = c.Int(nullable: false, identity: true),
                        SortingOrder = c.Int(nullable: false),
                        LeaveCategoryName = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LeaveCategoryId);
            
            CreateTable(
                "dbo.OnCallPhones",
                c => new
                    {
                        OnCallPhoneId = c.Int(nullable: false, identity: true),
                        SortingOrder = c.Int(nullable: false),
                        Title = c.String(),
                        PhoneNumber = c.String(),
                    })
                .PrimaryKey(t => t.OnCallPhoneId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(),
                    })
                .PrimaryKey(t => t.ProjectId);
            
            CreateTable(
                "dbo.WorkingLogs",
                c => new
                    {
                        WorkingLogId = c.Int(nullable: false, identity: true),
                        LogDate = c.DateTime(nullable: false),
                        TotalHours = c.Time(nullable: false, precision: 7),
                        Title = c.String(),
                        Summary = c.String(),
                        Owner_MyUserId = c.Int(),
                        Project_ProjectId = c.Int(),
                    })
                .PrimaryKey(t => t.WorkingLogId)
                .ForeignKey("dbo.MyUsers", t => t.Owner_MyUserId)
                .ForeignKey("dbo.Projects", t => t.Project_ProjectId)
                .Index(t => t.Owner_MyUserId)
                .Index(t => t.Project_ProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkingLogs", "Project_ProjectId", "dbo.Projects");
            DropForeignKey("dbo.WorkingLogs", "Owner_MyUserId", "dbo.MyUsers");
            DropForeignKey("dbo.LeaveAppForms", "Owner_MyUserId", "dbo.MyUsers");
            DropForeignKey("dbo.MyUsers", "Manager_MyUserId", "dbo.MyUsers");
            DropIndex("dbo.WorkingLogs", new[] { "Project_ProjectId" });
            DropIndex("dbo.WorkingLogs", new[] { "Owner_MyUserId" });
            DropIndex("dbo.MyUsers", new[] { "Manager_MyUserId" });
            DropIndex("dbo.LeaveAppForms", new[] { "Owner_MyUserId" });
            DropTable("dbo.WorkingLogs");
            DropTable("dbo.Projects");
            DropTable("dbo.OnCallPhones");
            DropTable("dbo.LeaveCategories");
            DropTable("dbo.MyUsers");
            DropTable("dbo.LeaveAppForms");
        }
    }
}
