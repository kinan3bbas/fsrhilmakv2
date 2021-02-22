namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScheduledJObs2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScheduledJobLogs", "error", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScheduledJobLogs", "error");
        }
    }
}
