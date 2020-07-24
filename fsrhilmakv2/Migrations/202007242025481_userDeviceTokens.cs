namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userDeviceTokens : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UsersDeviceTokens",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        token = c.String(),
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
            DropForeignKey("dbo.UsersDeviceTokens", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UsersDeviceTokens", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.UsersDeviceTokens", new[] { "ModifierId" });
            DropIndex("dbo.UsersDeviceTokens", new[] { "CreatorId" });
            DropTable("dbo.UsersDeviceTokens");
        }
    }
}
