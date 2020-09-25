namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class competition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Competitions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        Goal = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        UserWorkId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        CreatorId = c.String(maxLength: 128),
                        ModifierId = c.String(maxLength: 128),
                        AttachmentId = c.Long(nullable: false),
                        CreatorName = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifierId)
                .ForeignKey("dbo.UserWorks", t => t.UserWorkId, cascadeDelete: true)
                .Index(t => t.UserWorkId)
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Competitions", "UserWorkId", "dbo.UserWorks");
            DropForeignKey("dbo.Competitions", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Competitions", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.Competitions", new[] { "ModifierId" });
            DropIndex("dbo.Competitions", new[] { "CreatorId" });
            DropIndex("dbo.Competitions", new[] { "UserWorkId" });
            DropTable("dbo.Competitions");
        }
    }
}
