namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "SocialState", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SocialState");
        }
    }
}
