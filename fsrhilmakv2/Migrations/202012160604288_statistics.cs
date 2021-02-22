namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class statistics : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Statistics",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        AllServices = c.Int(nullable: false),
                        AllDoneServices = c.Int(nullable: false),
                        AllActiveServices = c.Int(nullable: false),
                        AllServiceProviders = c.Int(nullable: false),
                        AllInterpreters = c.Int(nullable: false),
                        AllIftaa = c.Int(nullable: false),
                        AllRouqat = c.Int(nullable: false),
                        AllMustashareen = c.Int(nullable: false),
                        AllClients = c.Int(nullable: false),
                        AllActiveClients = c.Int(nullable: false),
                        AllActiveClientsInThePastThreeDays = c.Int(nullable: false),
                        AllUsers = c.Int(nullable: false),
                        AllDreamUsers = c.Int(nullable: false),
                        AllRouqiaUsers = c.Int(nullable: false),
                        AllIstasharaUsers = c.Int(nullable: false),
                        AllMedicalUsers = c.Int(nullable: false),
                        AllIftaaUsers = c.Int(nullable: false),
                        AllLawUsers = c.Int(nullable: false),
                        status = c.String(),
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
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Statistics", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Statistics", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.Statistics", new[] { "ModifierId" });
            DropIndex("dbo.Statistics", new[] { "CreatorId" });
            DropTable("dbo.Statistics");
        }
    }
}
