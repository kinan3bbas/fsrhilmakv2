namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userCreationDateNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "CreationDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AspNetUsers", "LastModificationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "LastModificationDate", c => c.DateTime());
            AlterColumn("dbo.AspNetUsers", "CreationDate", c => c.DateTime());
        }
    }
}
