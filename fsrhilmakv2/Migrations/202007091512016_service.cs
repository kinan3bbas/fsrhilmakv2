namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class service : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        Description = c.String(),
                        ServiceProviderId = c.String(maxLength: 128),
                        ServicePathId = c.Int(nullable: false),
                        Explanation = c.String(),
                        ExplanationDate = c.DateTime(),
                        numberOfViews = c.Long(nullable: false),
                        numberOfLikes = c.Long(nullable: false),
                        Name = c.String(),
                        Sex = c.String(),
                        KidsStatus = c.String(),
                        SocialStatus = c.String(),
                        IsThereWakefulness = c.Boolean(nullable: false),
                        DreamDate = c.DateTime(),
                        Country = c.String(),
                        HaveYouPrayedBeforeTheDream = c.Boolean(nullable: false),
                        RegligionStatus = c.String(),
                        DidYouExorcism = c.Boolean(nullable: false),
                        PublicServiceAction = c.Boolean(nullable: false),
                        PrivateService = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        CreatorId = c.String(maxLength: 128),
                        ModifierId = c.String(maxLength: 128),
                        AttachmentId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifierId)
                .ForeignKey("dbo.ServicePaths", t => t.ServicePathId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ServiceProviderId)
                .Index(t => t.ServiceProviderId)
                .Index(t => t.ServicePathId)
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Services", "ServiceProviderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Services", "ServicePathId", "dbo.ServicePaths");
            DropForeignKey("dbo.Services", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Services", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.Services", new[] { "ModifierId" });
            DropIndex("dbo.Services", new[] { "CreatorId" });
            DropIndex("dbo.Services", new[] { "ServicePathId" });
            DropIndex("dbo.Services", new[] { "ServiceProviderId" });
            DropTable("dbo.Services");
        }
    }
}
