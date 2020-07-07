namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserWork : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserWorks",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AdjectiveName = c.String(),
                        Enabled = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        CreatorId = c.String(maxLength: 128),
                        ModifierId = c.String(maxLength: 128),
                        AttachmentId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifierId)
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserWorks", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserWorks", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.UserWorks", new[] { "ModifierId" });
            DropIndex("dbo.UserWorks", new[] { "CreatorId" });
            DropTable("dbo.UserWorks");
        }
    }
}
