namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceProviderRank : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "rank", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "rank");
        }
    }
}
