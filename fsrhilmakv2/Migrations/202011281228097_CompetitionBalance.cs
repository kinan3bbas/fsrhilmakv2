namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompetitionBalance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompetitionBalances",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        ServiceProviderId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                        Method = c.String(),
                        Currency = c.String(),
                        Status = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        CreatorId = c.String(maxLength: 128),
                        ModifierId = c.String(maxLength: 128),
                        AttachmentId = c.Long(nullable: false),
                        CreatorName = c.String(),
                        ServiceProvider_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifierId)
                .ForeignKey("dbo.AspNetUsers", t => t.ServiceProvider_Id)
                .Index(t => t.CreatorId)
                .Index(t => t.ModifierId)
                .Index(t => t.ServiceProvider_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompetitionBalances", "ServiceProvider_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompetitionBalances", "ModifierId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompetitionBalances", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.CompetitionBalances", new[] { "ServiceProvider_Id" });
            DropIndex("dbo.CompetitionBalances", new[] { "ModifierId" });
            DropIndex("dbo.CompetitionBalances", new[] { "CreatorId" });
            DropTable("dbo.CompetitionBalances");
        }
    }
}
