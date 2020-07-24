namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Payment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        ServiceId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                        Method = c.String(),
                        Currency = c.String(),
                        Status = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        CreatorId = c.String(maxLength: 128),
                        ModifierId = c.String(maxLength: 128),
                        AttachmentId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifierId)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId)
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.Payments", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Payments", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.Payments", new[] { "ModifierId" });
            DropIndex("dbo.Payments", new[] { "CreatorId" });
            DropIndex("dbo.Payments", new[] { "ServiceId" });
            DropTable("dbo.Payments");
        }
    }
}
