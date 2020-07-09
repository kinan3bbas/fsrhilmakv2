namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class serviceProvider : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServicePaths",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Cost = c.Double(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        IsPrivate = c.Boolean(nullable: false),
                        ServiceProviderId = c.String(maxLength: 128),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        CreatorId = c.String(maxLength: 128),
                        ModifierId = c.String(maxLength: 128),
                        AttachmentId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifierId)
                .ForeignKey("dbo.AspNetUsers", t => t.ServiceProviderId)
                .Index(t => t.ServiceProviderId)
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServicePaths", "ServiceProviderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ServicePaths", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ServicePaths", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.ServicePaths", new[] { "ModifierId" });
            DropIndex("dbo.ServicePaths", new[] { "CreatorId" });
            DropIndex("dbo.ServicePaths", new[] { "ServiceProviderId" });
            DropTable("dbo.ServicePaths");
        }
    }
}
