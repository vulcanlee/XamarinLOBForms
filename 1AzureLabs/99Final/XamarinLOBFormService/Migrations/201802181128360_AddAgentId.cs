namespace XamarinLOBFormService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAgentId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeaveAppForms", "AgentId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LeaveAppForms", "AgentId");
        }
    }
}
