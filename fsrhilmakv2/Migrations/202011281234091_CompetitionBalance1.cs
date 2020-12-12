namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompetitionBalance1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CompetitionBalances", new[] { "ServiceProvider_Id" });
            DropColumn("dbo.CompetitionBalances", "ServiceProviderId");
            RenameColumn(table: "dbo.CompetitionBalances", name: "ServiceProvider_Id", newName: "ServiceProviderId");
            AlterColumn("dbo.CompetitionBalances", "ServiceProviderId", c => c.String(maxLength: 128));
            CreateIndex("dbo.CompetitionBalances", "ServiceProviderId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.CompetitionBalances", new[] { "ServiceProviderId" });
            AlterColumn("dbo.CompetitionBalances", "ServiceProviderId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.CompetitionBalances", name: "ServiceProviderId", newName: "ServiceProvider_Id");
            AddColumn("dbo.CompetitionBalances", "ServiceProviderId", c => c.Int(nullable: false));
            CreateIndex("dbo.CompetitionBalances", "ServiceProvider_Id");
        }
    }
}
