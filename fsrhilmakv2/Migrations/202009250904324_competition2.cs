namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class competition2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Competitions", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Competitions", "Name");
        }
    }
}
