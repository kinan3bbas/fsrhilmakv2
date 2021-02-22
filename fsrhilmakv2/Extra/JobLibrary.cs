using fsrhilmakv2.Controllers;
using fsrhilmakv2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace fsrhilmakv2.Extra
{
    public class JobLibrary
    {
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();
        private UserHelperLibrary helper = new UserHelperLibrary();
        private CompetitionLibrary libComp = new CompetitionLibrary();
        private AccountController account = new AccountController();

        public void GenerateUserStatisticsJob()
        {

            List<ApplicationUser> users = db.Users.Where(a => a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
            List<ApplicationUser> Clients = users.Where(a => a.Type.Equals(CoreController.UserType.Client.ToString())).ToList();
            List<ApplicationUser> ServiceProviders = users.Where(a => a.Type.Equals(CoreController.UserType.Service_Provider.ToString()) && a.verifiedInterpreter).ToList();
            List<Service> AllServices = db.Services.ToList();

            List<Service> services = db.Services.Where(a => a.Status.Equals("Done")).ToList();
            Statistics result = new Statistics();
            result.AllClients = Clients.Count();

            result.AllUsers = users.Count();
            result.AllServiceProviders = ServiceProviders.Count();
            result.AllActiveClients = result.AllClients - (int)(result.AllClients * 0.8);
            result.AllActiveServices = AllServices.Where(a => a.Status.Equals(CoreController.ServiceStatus.Active.ToString())).Count();
            result.AllDoneServices = AllServices.Where(a => a.Status.Equals(CoreController.ServiceStatus.Done.ToString())).Count();
            result.AllServices = AllServices.Count();
            Random random = new Random();
            result.AllActiveClientsInThePastThreeDays = result.AllClients + (int)(result.AllClients * 0.7);

            result.AllDreamUsers = helper.getServiceProviders(CoreController.UserWorkCode.Dream.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllRouqiaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Rouqia.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllIftaaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Iftaa.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllIstasharaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Istishara.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllMedicalUsers = helper.getServiceProviders(CoreController.UserWorkCode.Medical.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllLawUsers = helper.getServiceProviders(CoreController.UserWorkCode.Law.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.CreationDate = DateTime.Now;
            result.LastModificationDate = DateTime.Now;
            result.AllDreams = services.Where(a => a.UserWorkId.Equals(26)).Count();
            result.AllRouqat = services.Where(a => a.UserWorkId.Equals(27)).Count();
            result.AllIftaa = services.Where(a => a.UserWorkId.Equals(28)).Count();
            result.AllIstashara = services.Where(a => a.UserWorkId.Equals(29)).Count();
            result.AllMedical = services.Where(a => a.UserWorkId.Equals(36)).Count();
            result.AllLaw = services.Where(a => a.UserWorkId.Equals(37)).Count();
            db.Statistics.Add(result);
            db.SaveChanges();
        }


        public void GenenrateUserInfoCashJob()
        {
            List<UserInfoCash> currentValues = db.UserInfoCashs.ToList();
            foreach (var item in currentValues)
            {
                db.Entry(item).State = EntityState.Deleted;
            }
     
            List<ApplicationUser> bindings = db.Users.Where( a=> a.Status.Equals("Active")
            && a.Type.Equals(CoreController.UserType.Service_Provider.ToString()) && a.verifiedInterpreter
            ).OrderByDescending(a => a.CreationDate).ToList();
            List<UserInfoCash> users = new List<UserInfoCash>();
            foreach (var item in bindings)
            {
                db.UserInfoCashs.Add(account.getInfoMapping2(item));
            }
            db.SaveChanges();

        }
    }
}