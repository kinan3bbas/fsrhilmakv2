namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userpicture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfoCashes", "PictureFileName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserInfoCashes", "PictureFileName");
        }
    }
}
