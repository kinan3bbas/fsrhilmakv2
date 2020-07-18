namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ratingMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "RatingMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "RatingMessage");
        }
    }
}
