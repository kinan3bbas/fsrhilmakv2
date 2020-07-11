namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jobstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "JobStatus", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "JobStatus");
        }
    }
}
