namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class compittion3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompetitionPrizes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        CompetitionId = c.Int(nullable: false),
                        rank = c.Int(nullable: false),
                        price = c.Double(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        CreatorId = c.String(maxLength: 128),
                        ModifierId = c.String(maxLength: 128),
                        AttachmentId = c.Long(nullable: false),
                        CreatorName = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Competitions", t => t.CompetitionId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifierId)
                .Index(t => t.CompetitionId)
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompetitionPrizes", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompetitionPrizes", "CreatorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompetitionPrizes", "CompetitionId", "dbo.Competitions");
            DropIndex("dbo.CompetitionPrizes", new[] { "ModifierId" });
            DropIndex("dbo.CompetitionPrizes", new[] { "CreatorId" });
            DropIndex("dbo.CompetitionPrizes", new[] { "CompetitionId" });
            DropTable("dbo.CompetitionPrizes");
        }
    }
}
