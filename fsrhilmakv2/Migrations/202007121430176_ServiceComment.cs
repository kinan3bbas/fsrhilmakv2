namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceComment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceComments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
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
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId)
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceComments", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceComments", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ServiceComments", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.ServiceComments", new[] { "ModifierId" });
            DropIndex("dbo.ServiceComments", new[] { "CreatorId" });
            DropIndex("dbo.ServiceComments", new[] { "ServiceId" });
            DropTable("dbo.ServiceComments");
        }
    }
}
