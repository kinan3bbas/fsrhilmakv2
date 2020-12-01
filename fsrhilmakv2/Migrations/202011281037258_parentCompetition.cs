namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class parentCompetition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Competitions", "ParentCompetitionId", c => c.Int(nullable: false));
            CreateIndex("dbo.Competitions", "ParentCompetitionId");
            AddForeignKey("dbo.Competitions", "ParentCompetitionId", "dbo.Competitions", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Competitions", "ParentCompetitionId", "dbo.Competitions");
            DropIndex("dbo.Competitions", new[] { "ParentCompetitionId" });
            DropColumn("dbo.Competitions", "ParentCompetitionId");
        }
    }
}
