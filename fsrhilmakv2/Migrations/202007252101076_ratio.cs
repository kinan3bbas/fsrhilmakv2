namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ratio : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServicePaths", "Ratio", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServicePaths", "Ratio");
        }
    }
}
