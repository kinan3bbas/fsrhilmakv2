namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatebinding : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserWorkBindings", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserWorkBindings", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserWorkBindings", new[] { "UserId" });
            DropIndex("dbo.UserWorkBindings", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.UserWorkBindings", "UserId");
            RenameColumn(table: "dbo.UserWorkBindings", name: "ApplicationUser_Id", newName: "UserId");
            AlterColumn("dbo.UserWorkBindings", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.UserWorkBindings", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.UserWorkBindings", "UserId");
            AddForeignKey("dbo.UserWorkBindings", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserWorkBindings", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserWorkBindings", new[] { "UserId" });
            AlterColumn("dbo.UserWorkBindings", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.UserWorkBindings", "UserId", c => c.String(maxLength: 128));
            RenameColumn(table: "dbo.UserWorkBindings", name: "UserId", newName: "ApplicationUser_Id");
            AddColumn("dbo.UserWorkBindings", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.UserWorkBindings", "ApplicationUser_Id");
            CreateIndex("dbo.UserWorkBindings", "UserId");
            AddForeignKey("dbo.UserWorkBindings", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.UserWorkBindings", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
