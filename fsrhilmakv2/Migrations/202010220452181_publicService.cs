namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class publicService : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PublicServices",
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
                        PrivateServicePrice = c.Double(nullable: false),
                        UserWorkId = c.Int(nullable: false),
                        JobStatus = c.String(),
                        NumberOfRemainingPeople = c.Long(nullable: false),
                        NumberOfAllPeopleWaiting = c.Long(nullable: false),
                        AvgWaitingTime = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        CreatorId = c.String(maxLength: 128),
                        ModifierId = c.String(maxLength: 128),
                        AttachmentId = c.Long(nullable: false),
                        ServiceProviderToken = c.String(),
                        ClientToken = c.String(),
                        ServiceProviderSpeed = c.Double(nullable: false),
                        ServiceProviderAvgServices = c.Double(nullable: false),
                        ServiceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifierId)
                .ForeignKey("dbo.ServicePaths", t => t.ServicePathId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ServiceProviderId)
                .ForeignKey("dbo.UserWorks", t => t.UserWorkId, cascadeDelete: true)
                .Index(t => t.ServiceProviderId)
                .Index(t => t.ServicePathId)
                .Index(t => t.UserWorkId)
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId);
            
            AddColumn("dbo.ServiceComments", "PublicService_id", c => c.Int());
            CreateIndex("dbo.ServiceComments", "PublicService_id");
            AddForeignKey("dbo.ServiceComments", "PublicService_id", "dbo.PublicServices", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PublicServices", "UserWorkId", "dbo.UserWorks");
            DropForeignKey("dbo.PublicServices", "ServiceProviderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PublicServices", "ServicePathId", "dbo.ServicePaths");
            DropForeignKey("dbo.PublicServices", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PublicServices", "CreatorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ServiceComments", "PublicService_id", "dbo.PublicServices");
            DropIndex("dbo.PublicServices", new[] { "ModifierId" });
            DropIndex("dbo.PublicServices", new[] { "CreatorId" });
            DropIndex("dbo.PublicServices", new[] { "UserWorkId" });
            DropIndex("dbo.PublicServices", new[] { "ServicePathId" });
            DropIndex("dbo.PublicServices", new[] { "ServiceProviderId" });
            DropIndex("dbo.ServiceComments", new[] { "PublicService_id" });
            DropColumn("dbo.ServiceComments", "PublicService_id");
            DropTable("dbo.PublicServices");
        }
    }
}
