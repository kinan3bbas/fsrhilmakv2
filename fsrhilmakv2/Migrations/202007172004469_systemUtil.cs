namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class systemUtil : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Sender = c.String(nullable: false),
                        Receiver = c.String(nullable: false),
                        Subject = c.String(nullable: false),
                        Body = c.String(),
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
            
            CreateTable(
                "dbo.UserVerificationLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        ConfirmationDate = c.DateTime(),
                        ExpiryDate = c.DateTime(nullable: false),
                        Code = c.String(),
                        Status = c.String(),
                        Email = c.String(),
                        IsEmailSent = c.Boolean(nullable: false),
                        UserId = c.String(),
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
            DropForeignKey("dbo.UserVerificationLogs", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserVerificationLogs", "CreatorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailLogs", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmailLogs", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.UserVerificationLogs", new[] { "ModifierId" });
            DropIndex("dbo.UserVerificationLogs", new[] { "CreatorId" });
            DropIndex("dbo.EmailLogs", new[] { "ModifierId" });
            DropIndex("dbo.EmailLogs", new[] { "CreatorId" });
            DropTable("dbo.UserVerificationLogs");
            DropTable("dbo.EmailLogs");
        }
    }
}
