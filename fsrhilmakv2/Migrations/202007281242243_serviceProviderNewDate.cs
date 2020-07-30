namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class serviceProviderNewDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "ServiceProviderNewDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "ServiceProviderNewDate");
        }
    }
}
