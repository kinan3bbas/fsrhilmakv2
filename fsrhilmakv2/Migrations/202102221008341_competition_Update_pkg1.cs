namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class competition_Update_pkg1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Competitions", "EndDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Competitions", "EndDate", c => c.DateTime());
        }
    }
}
