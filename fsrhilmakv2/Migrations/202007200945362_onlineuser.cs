namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onlineuser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Online", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Online");
        }
    }
}
