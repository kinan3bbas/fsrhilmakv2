namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class publicService1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiceComments", "PublicService_id", "dbo.PublicServices");
            DropIndex("dbo.ServiceComments", new[] { "PublicService_id" });
            DropColumn("dbo.ServiceComments", "PublicService_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ServiceComments", "PublicService_id", c => c.Int());
            CreateIndex("dbo.ServiceComments", "PublicService_id");
            AddForeignKey("dbo.ServiceComments", "PublicService_id", "dbo.PublicServices", "id");
        }
    }
}
