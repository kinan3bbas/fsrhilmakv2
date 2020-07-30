namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class serviceProviderNewDate2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Services", "ServiceProviderNewDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Services", "ServiceProviderNewDate", c => c.DateTime());
        }
    }
}
