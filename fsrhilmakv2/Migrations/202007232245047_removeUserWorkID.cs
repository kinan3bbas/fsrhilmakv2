namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeUserWorkID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserWorkBindings", "UserWorkId", "dbo.UserWorks");
            DropIndex("dbo.UserWorkBindings", new[] { "UserWorkId" });
            RenameColumn(table: "dbo.UserWorkBindings", name: "UserWorkId", newName: "UserWork_id");
            AlterColumn("dbo.UserWorkBindings", "UserWork_id", c => c.Int());
            CreateIndex("dbo.UserWorkBindings", "UserWork_id");
            AddForeignKey("dbo.UserWorkBindings", "UserWork_id", "dbo.UserWorks", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserWorkBindings", "UserWork_id", "dbo.UserWorks");
            DropIndex("dbo.UserWorkBindings", new[] { "UserWork_id" });
            AlterColumn("dbo.UserWorkBindings", "UserWork_id", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.UserWorkBindings", name: "UserWork_id", newName: "UserWorkId");
            CreateIndex("dbo.UserWorkBindings", "UserWorkId");
            AddForeignKey("dbo.UserWorkBindings", "UserWorkId", "dbo.UserWorks", "id", cascadeDelete: true);
        }
    }
}
