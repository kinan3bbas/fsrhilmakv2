namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServicePathMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServicePaths", "Message", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServicePaths", "Message");
        }
    }
}
