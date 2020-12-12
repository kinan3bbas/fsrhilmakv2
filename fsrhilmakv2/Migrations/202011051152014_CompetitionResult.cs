namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompetitionResult : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompetitionResults",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        NumberOfActiveServices = c.Int(nullable: false),
                        NumberOfDoneServices = c.Int(nullable: false),
                        Speed = c.Double(nullable: false),
                        TotalBalance = c.Double(nullable: false),
                        SuspendedBalance = c.Double(nullable: false),
                        AvailableBalance = c.Double(nullable: false),
                        AvgServicesInOneDay = c.Double(nullable: false),
                        Rating = c.Double(nullable: false),
                        ServiceProviderId = c.String(maxLength: 128),
                        competitionId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        CreatorId = c.String(maxLength: 128),
                        ModifierId = c.String(maxLength: 128),
                        AttachmentId = c.Long(nullable: false),
                        CreatorName = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Competitions", t => t.competitionId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifierId)
                .ForeignKey("dbo.AspNetUsers", t => t.ServiceProviderId)
                .Index(t => t.ServiceProviderId)
                .Index(t => t.competitionId)
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompetitionResults", "ServiceProviderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompetitionResults", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompetitionResults", "CreatorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompetitionResults", "competitionId", "dbo.Competitions");
            DropIndex("dbo.CompetitionResults", new[] { "ModifierId" });
            DropIndex("dbo.CompetitionResults", new[] { "CreatorId" });
            DropIndex("dbo.CompetitionResults", new[] { "competitionId" });
            DropIndex("dbo.CompetitionResults", new[] { "ServiceProviderId" });
            DropTable("dbo.CompetitionResults");
        }
    }
}
