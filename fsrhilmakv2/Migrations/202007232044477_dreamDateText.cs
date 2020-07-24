namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dreamDateText : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "DreamDateText", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "DreamDateText");
        }
    }
}
