namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newFIx__return : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserWorkBindings", "UserWork_id", "dbo.UserWorks");
            DropIndex("dbo.UserWorkBindings", new[] { "UserWork_id" });
            RenameColumn(table: "dbo.UserWorkBindings", name: "UserWork_id", newName: "UserWorkId");
            AlterColumn("dbo.UserWorkBindings", "UserWorkId", c => c.Int(nullable: false));
            CreateIndex("dbo.UserWorkBindings", "UserWorkId");
            AddForeignKey("dbo.UserWorkBindings", "UserWorkId", "dbo.UserWorks", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserWorkBindings", "UserWorkId", "dbo.UserWorks");
            DropIndex("dbo.UserWorkBindings", new[] { "UserWorkId" });
            AlterColumn("dbo.UserWorkBindings", "UserWorkId", c => c.Int());
            RenameColumn(table: "dbo.UserWorkBindings", name: "UserWorkId", newName: "UserWork_id");
            CreateIndex("dbo.UserWorkBindings", "UserWork_id");
            AddForeignKey("dbo.UserWorkBindings", "UserWork_id", "dbo.UserWorks", "id");
        }
    }
}
