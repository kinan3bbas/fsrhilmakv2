namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class serviceProvider2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServicePaths", "ServiceProviderId", c => c.String(maxLength: 128));
            CreateIndex("dbo.ServicePaths", "ServiceProviderId");
            AddForeignKey("dbo.ServicePaths", "ServiceProviderId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServicePaths", "ServiceProviderId", "dbo.AspNetUsers");
            DropIndex("dbo.ServicePaths", new[] { "ServiceProviderId" });
            DropColumn("dbo.ServicePaths", "ServiceProviderId");
        }
    }
}
