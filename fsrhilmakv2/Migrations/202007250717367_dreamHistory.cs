namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dreamHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DreamHistories",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        OldInterpreterId = c.String(maxLength: 128),
                        NewInterpreterId = c.String(maxLength: 128),
                        ServiceId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        CreatorId = c.String(maxLength: 128),
                        ModifierId = c.String(maxLength: 128),
                        AttachmentId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifierId)
                .ForeignKey("dbo.AspNetUsers", t => t.NewInterpreterId)
                .ForeignKey("dbo.AspNetUsers", t => t.OldInterpreterId)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.OldInterpreterId)
                .Index(t => t.NewInterpreterId)
                .Index(t => t.ServiceId)
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DreamHistories", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.DreamHistories", "OldInterpreterId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DreamHistories", "NewInterpreterId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DreamHistories", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DreamHistories", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.DreamHistories", new[] { "ModifierId" });
            DropIndex("dbo.DreamHistories", new[] { "CreatorId" });
            DropIndex("dbo.DreamHistories", new[] { "ServiceId" });
            DropIndex("dbo.DreamHistories", new[] { "NewInterpreterId" });
            DropIndex("dbo.DreamHistories", new[] { "OldInterpreterId" });
            DropTable("dbo.DreamHistories");
        }
    }
}
