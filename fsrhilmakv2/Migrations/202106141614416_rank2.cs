namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rank2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfoCashes", "rank", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserInfoCashes", "rank");
        }
    }
}
