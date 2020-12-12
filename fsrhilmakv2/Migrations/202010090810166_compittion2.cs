namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class compittion2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Competitions", "duration", c => c.Int(nullable: false));
            AddColumn("dbo.Competitions", "repeat", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Competitions", "repeat");
            DropColumn("dbo.Competitions", "duration");
        }
    }
}
