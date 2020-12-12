using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ControlPanel.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {


        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Last Modification Date")]
        public DateTime LastModificationDate { get; set; }
        [Display(Name = "Sex")]
        public string Sex { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Martial Status")]
        public string MartialStatus { get; set; }

        [Display(Name = "Job Description")]
        public string JobDescription { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Joining Date")]
        public DateTime? JoiningDate { get; set; }

        [Display(Name = "Picture Id")]
        public string PictureId { get; set; }

        public int numbOfDreamsInOneDay { get; set; }

        //public ICollection<Dream> Dreams { get; set; }

        public int Age { get; set; }

        public string FireBaseId { get; set; }

        public string PersonalDescription { get; set; }

        public bool verifiedInterpreter { get; set; }

        public string SocialState { get; set; }

        public ICollection<UserWorkBinding> userWorkBinding { get; set; }

        public String UserSpecialCode { get; set; }

        public String UserRegistrationCode { get; set; }

        public long PointsBalance { get; set; }

        public bool Online { get; set; }

        public String imageUrl { get; set; }

        public String SocialToken { get; set; }

        public long ServiceProviderPoints { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // configures one-to-many relationship
            modelBuilder.Entity<UserWorkBinding>()
                .HasRequired<ApplicationUser>(s => s.User)
                .WithMany(g => g.userWorkBinding)
                .HasForeignKey<string>(s => s.UserId);


            modelBuilder.Entity<ServiceComment>()
                .HasRequired<Service>(s => s.Service)
                .WithMany(g => g.Comments)
                .HasForeignKey<int>(s => s.ServiceId);

            //modelBuilder.Entity<CompetitionPrize>()
            //    .HasRequired<Competition>(s => s.competition)
            //    .WithMany(g => g.prizes)
            //    .HasForeignKey<int>(s => s.CompetitionId);
            //modelBuilder.Entity<Competition>().HasOptional<Competition>(g => g.ParentCompetition);
            base.OnModelCreating(modelBuilder);
        }

        public System.Data.Entity.DbSet<ControlPanel.Models.UserWork> UserWorks { get; set; }

        public System.Data.Entity.DbSet<ControlPanel.Models.SystemParameter> SystemParameters { get; set; }

        public System.Data.Entity.DbSet<ControlPanel.Models.ServicePath> ServicePaths { get; set; }

        public System.Data.Entity.DbSet<ControlPanel.Models.UserWorkBinding> UserWorkBindings { get; set; }

        public System.Data.Entity.DbSet<Service> Services { get; set; }
        public System.Data.Entity.DbSet<ServiceComment> ServiceComments { get; set; }
        public System.Data.Entity.DbSet<UserVerificationLog> UserVerificationLogs { get; set; }
        public System.Data.Entity.DbSet<EmailLog> EmailLogs { get; set; }

        public System.Data.Entity.DbSet<Payment> Payments { get; set; }

        
        public System.Data.Entity.DbSet<DreamHistory> DreamHistorys { get; set; }

        public System.Data.Entity.DbSet<Transaction> Transactions { get; set; }

        public System.Data.Entity.DbSet<Competition> Competitions { get; set; }

        public System.Data.Entity.DbSet<CompetitionPrize> CompetitionPrizes { get; set; }


        public System.Data.Entity.DbSet<PublicService> PublicServices { get; set; }

        public System.Data.Entity.DbSet<CompetitionResult> CompetitionResults { get; set; }

        public System.Data.Entity.DbSet<CompetitionBalance> CompetitionBalances { get; set; }

    }
}