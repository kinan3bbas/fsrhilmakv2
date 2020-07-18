namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UserSpecialCode", c => c.String());
            AddColumn("dbo.AspNetUsers", "UserRegistrationCode", c => c.String());
            AddColumn("dbo.AspNetUsers", "PointsBalance", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "PointsBalance");
            DropColumn("dbo.AspNetUsers", "UserRegistrationCode");
            DropColumn("dbo.AspNetUsers", "UserSpecialCode");
        }
    }
}
