namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class service3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "PrivateServicePrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "PrivateServicePrice");
        }
    }
}
