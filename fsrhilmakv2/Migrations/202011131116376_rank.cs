namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rank : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompetitionResults", "rank", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompetitionResults", "rank");
        }
    }
}
