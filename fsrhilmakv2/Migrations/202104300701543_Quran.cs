namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Quran : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Qurans",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        Number = c.Int(nullable: false),
                        Surat = c.String(),
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
            DropForeignKey("dbo.Qurans", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Qurans", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.Qurans", new[] { "ModifierId" });
            DropIndex("dbo.Qurans", new[] { "CreatorId" });
            DropTable("dbo.Qurans");
        }
    }
}
