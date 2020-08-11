namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class services3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Services", "ServiceProviderId", "dbo.AspNetUsers");
            DropIndex("dbo.Services", new[] { "ServiceProviderId" });
            AlterColumn("dbo.Services", "ServiceProviderId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Services", "ServiceProviderId");
            AddForeignKey("dbo.Services", "ServiceProviderId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Services", "ServiceProviderId", "dbo.AspNetUsers");
            DropIndex("dbo.Services", new[] { "ServiceProviderId" });
            AlterColumn("dbo.Services", "ServiceProviderId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Services", "ServiceProviderId");
            AddForeignKey("dbo.Services", "ServiceProviderId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
