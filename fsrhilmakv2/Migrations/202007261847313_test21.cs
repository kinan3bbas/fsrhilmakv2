namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test21 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserWorks", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserWorks", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.UserWorks", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserWorks", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.UserWorks", "ApplicationUser_Id");
            AddForeignKey("dbo.UserWorks", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
