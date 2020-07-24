namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class socialUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "imageUrl", c => c.String());
            AddColumn("dbo.AspNetUsers", "SocialToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SocialToken");
            DropColumn("dbo.AspNetUsers", "imageUrl");
        }
    }
}
