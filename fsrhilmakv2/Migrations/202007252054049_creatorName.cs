namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class creatorName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DreamHistories", "CreatorName", c => c.String());
            AddColumn("dbo.UserWorkBindings", "CreatorName", c => c.String());
            AddColumn("dbo.Services", "CreatorName", c => c.String());
            AddColumn("dbo.ServiceComments", "CreatorName", c => c.String());
            AddColumn("dbo.ServicePaths", "CreatorName", c => c.String());
            AddColumn("dbo.EmailLogs", "CreatorName", c => c.String());
            AddColumn("dbo.Payments", "CreatorName", c => c.String());
            AddColumn("dbo.UsersDeviceTokens", "CreatorName", c => c.String());
            AddColumn("dbo.UserVerificationLogs", "CreatorName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserVerificationLogs", "CreatorName");
            DropColumn("dbo.UsersDeviceTokens", "CreatorName");
            DropColumn("dbo.Payments", "CreatorName");
            DropColumn("dbo.EmailLogs", "CreatorName");
            DropColumn("dbo.ServicePaths", "CreatorName");
            DropColumn("dbo.ServiceComments", "CreatorName");
            DropColumn("dbo.Services", "CreatorName");
            DropColumn("dbo.UserWorkBindings", "CreatorName");
            DropColumn("dbo.DreamHistories", "CreatorName");
        }
    }
}
