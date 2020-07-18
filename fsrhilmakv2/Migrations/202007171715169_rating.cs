namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "UserRating", c => c.Int(nullable: false));
            AddColumn("dbo.Services", "RatingDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "RatingDate");
            DropColumn("dbo.Services", "UserRating");
        }
    }
}
