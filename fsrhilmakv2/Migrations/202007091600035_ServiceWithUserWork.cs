namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceWithUserWork : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "UserWorkId", c => c.Int(nullable: false));
            CreateIndex("dbo.Services", "UserWorkId");
            AddForeignKey("dbo.Services", "UserWorkId", "dbo.UserWorks", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Services", "UserWorkId", "dbo.UserWorks");
            DropIndex("dbo.Services", new[] { "UserWorkId" });
            DropColumn("dbo.Services", "UserWorkId");
        }
    }
}
