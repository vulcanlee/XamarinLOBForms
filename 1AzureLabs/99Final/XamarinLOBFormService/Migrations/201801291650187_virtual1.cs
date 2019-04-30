namespace XamarinLOBFormService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class virtual1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MyUsers", "Manager_MyUserId", "dbo.MyUsers");
            DropIndex("dbo.MyUsers", new[] { "Manager_MyUserId" });
            AddColumn("dbo.MyUsers", "ManagerId", c => c.Int(nullable: false));
            DropColumn("dbo.MyUsers", "Manager_MyUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MyUsers", "Manager_MyUserId", c => c.Int());
            DropColumn("dbo.MyUsers", "ManagerId");
            CreateIndex("dbo.MyUsers", "Manager_MyUserId");
            AddForeignKey("dbo.MyUsers", "Manager_MyUserId", "dbo.MyUsers", "MyUserId");
        }
    }
}
