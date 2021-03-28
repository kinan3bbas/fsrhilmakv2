using ControlPanel.Extra;
using ControlPanel.Extras;
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

            List<ApplicationUser> users = db.Users.Where(a => a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
            List<ApplicationUser> Clients = db.Users.Where(a => a.Type.Equals(CoreController.UserType.Client.ToString())&& 
                a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
            List<ApplicationUser> ServiceProviders = db.Users.Where(a => a.Type.Equals(CoreController.UserType.Service_Provider.ToString())&& 
                a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
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
            result.TotalBalance = 0.0;
            result.AvailableBalance = 0.0;
            result.SuspendedBalance = 0.0;
            if (ServiceProviders.Count > 0 && balancer.Count > 0)
            {
                foreach (var item in balancer)
                {
                    result.TotalBalance += (item.TransferedBalance != null ? item.TransferedBalance : 0.0);
                    result.AvailableBalance += (item.DoneBalance != null ? item.DoneBalance : 0.0);
                    result.SuspendedBalance += (item.SuspendedBalance != null ? item.SuspendedBalance : 0.0);
                }
                result.TotalBalance = balancer.Select(a => a.TransferedBalance).DefaultIfEmpty(0.0).Sum();
                result.AvailableBalance = balancer.Select(a => a.DoneBalance).DefaultIfEmpty(0.0).Sum();
                result.SuspendedBalance = balancer.Select(a => a.SuspendedBalance).DefaultIfEmpty(0.0).Sum();
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