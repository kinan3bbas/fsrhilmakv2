namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class parentCompetition1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Competitions", new[] { "ParentCompetitionId" });
            AlterColumn("dbo.Competitions", "ParentCompetitionId", c => c.Int());
            CreateIndex("dbo.Competitions", "ParentCompetitionId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Competitions", new[] { "ParentCompetitionId" });
            AlterColumn("dbo.Competitions", "ParentCompetitionId", c => c.Int(nullable: false));
            CreateIndex("dbo.Competitions", "ParentCompetitionId");
        }
    }
}
