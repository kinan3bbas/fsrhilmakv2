namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class statistics2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Statistics", "AllDreams", c => c.Int(nullable: false));
            AddColumn("dbo.Statistics", "AllLaw", c => c.Int(nullable: false));
            AddColumn("dbo.Statistics", "AllMedical", c => c.Int(nullable: false));
            AddColumn("dbo.Statistics", "AllIstashara", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Statistics", "AllIstashara");
            DropColumn("dbo.Statistics", "AllMedical");
            DropColumn("dbo.Statistics", "AllLaw");
            DropColumn("dbo.Statistics", "AllDreams");
        }
    }
}
