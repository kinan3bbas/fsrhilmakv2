namespace fsrhilmakv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userInfoCash : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserInfoCashes",
                c => new
                    {
                        idd = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        Email = c.String(),
                        PersonalDescription = c.String(),
                        HasRegistered = c.Boolean(nullable: false),
                        LoginProvider = c.String(),
                        Sex = c.String(),
                        Country = c.String(),
                        Name = c.String(),
                        Type = c.String(),
                        Status = c.String(),
                        MartialStatus = c.String(),
                        JobDescription = c.String(),
                        JoiningDate = c.DateTime(),
                        PictureId = c.String(),
                        AvgServicesInOneDay = c.Double(nullable: false),
                        Age = c.Int(nullable: false),
                        phoneNumber = c.String(),
                        FireBaseId = c.String(),
                        Id = c.String(),
                        NumberOfActiveServices = c.Int(nullable: false),
                        NumberOfDoneServices = c.Int(nullable: false),
                        SocialStatus = c.String(),
                        UserName = c.String(),
                        VerifiedUser = c.Boolean(nullable: false),
                        Speed = c.Double(nullable: false),
                        UserSpecialCode = c.String(),
                        UserRegistrationCode = c.String(),
                        PointsBalance = c.Long(nullable: false),
                        SocialToken = c.String(),
                        ImageUrl = c.String(),
                        TotalBalance = c.Double(nullable: false),
                        SuspendedBalance = c.Double(nullable: false),
                        AvailableBalance = c.Double(nullable: false),
                        ServiceProviderPoints = c.Long(nullable: false),
                        numberOfFreeServices = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.idd);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserInfoCashes");
        }
    }
}
