using ControlPanel.Models;
using ControlPanel.ViewModels;
using fsrhilmakv2.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private UserHelperLibrary helper = new UserHelperLibrary();
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();
        public ActionResult Index()
        {

            List<ApplicationUser> users = db.Users.Where(a => !a.Status.Equals(CoreController.UserStatus.Deleted.ToString())).ToList();
            List<ApplicationUser> Clients = db.Users.Where(a => a.Type.Equals(CoreController.UserType.Client.ToString())).ToList();
            List<ApplicationUser> ServiceProviders = db.Users.Where(a => a.Type.Equals(CoreController.UserType.Service_Provider.ToString())).ToList();
            List<Service> AllServices = db.Services.ToList();


            StatisticsViewModel result = new StatisticsViewModel();
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

            List<UserBalance> balancer = new List<UserBalance>();
            foreach (var item in ServiceProviders)
            {
                balancer.Add(helper.getUserBalance(item));
            }
            if (ServiceProviders.Count > 0) {
                result.TotalBalance = balancer.Sum(a => a.TransferedBalance);
                result.AvailableBalance = balancer.Sum(a => a.DoneBalance);
                result.SuspendedBalance = balancer.Sum(a => a.SuspendedBalance);
                result.AllPaymentsSum = db.Payments.Sum(a => a.Amount);
                result.Profit = result.AllPaymentsSum - (result.TotalBalance + result.AvailableBalance + result.SuspendedBalance);
            }
            
            return View(result);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}