namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScheduledJObs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScheduledJobLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        scheduledJobId = c.Int(nullable: false),
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
                .ForeignKey("dbo.ScheduledJobs", t => t.scheduledJobId, cascadeDelete: true)
                .Index(t => t.scheduledJobId)
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId);
            
            CreateTable(
                "dbo.ScheduledJobs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Code = c.String(nullable: false),
                        interval = c.Int(nullable: false),
                        Status = c.String(),
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
            DropForeignKey("dbo.ScheduledJobLogs", "scheduledJobId", "dbo.ScheduledJobs");
            DropForeignKey("dbo.ScheduledJobs", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ScheduledJobs", "CreatorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ScheduledJobLogs", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ScheduledJobLogs", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.ScheduledJobs", new[] { "ModifierId" });
            DropIndex("dbo.ScheduledJobs", new[] { "CreatorId" });
            DropIndex("dbo.ScheduledJobLogs", new[] { "ModifierId" });
            DropIndex("dbo.ScheduledJobLogs", new[] { "CreatorId" });
            DropIndex("dbo.ScheduledJobLogs", new[] { "scheduledJobId" });
            DropTable("dbo.ScheduledJobs");
            DropTable("dbo.ScheduledJobLogs");
        }
    }
}
