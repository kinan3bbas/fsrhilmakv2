namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class serviceProviderPoints : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ServiceProviderPoints", c => c.Long(nullable: false));
            AddColumn("dbo.CompetitionResults", "pointsBalance", c => c.Long(nullable: false));
            AddColumn("dbo.Competitions", "pointsBalance", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Competitions", "pointsBalance");
            DropColumn("dbo.CompetitionResults", "pointsBalance");
            DropColumn("dbo.AspNetUsers", "ServiceProviderPoints");
        }
    }
}
