using fsrhilmakv2.Extra;
using fsrhilmakv2.Models;
using fsrhilmakv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fsrhilmakv2.Controllers
{
    public class HomeController : Controller
    {
        private UserHelperLibrary helper = new UserHelperLibrary();
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Main()
        {
            Statistics result = db.Statistics.Where(a => a.status.Equals("Active")).FirstOrDefault();

            return View(result);
        }

        public ActionResult NewDesign()
        {
            Statistics result = db.Statistics.Where(a => a.status.Equals("Active")).FirstOrDefault();

            return View(result);
        }

        [HttpGet]
        public JsonResult generateStatisics()
        {
            List<ApplicationUser> users = db.Users.Where(a => a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
            List<ApplicationUser> Clients = db.Users.Where(a => a.Type.Equals(CoreController.UserType.Client.ToString()) &&
                a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
            List<ApplicationUser> ServiceProviders = db.Users.Where(a => a.Type.Equals(CoreController.UserType.Service_Provider.ToString()) &&
                a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
            List<Service> AllServices = db.Services.ToList();


            Statistics result = new Statistics();
            result.AllClients = Clients.Count();

            result.AllUsers = users.Count();
            result.AllServiceProviders = ServiceProviders.Count();
            result.AllActiveClients = result.AllClients - (int)(result.AllClients * 0.8);
            result.AllActiveServices = AllServices.Where(a => a.Status.Equals(CoreController.ServiceStatus.Active.ToString())).Count();
            result.AllDoneServices = AllServices.Where(a => a.Status.Equals(CoreController.ServiceStatus.Done.ToString())).Count();
            result.AllServices = AllServices.Count();
            Random random = new Random();
            //result.AllActiveClientsInThePastThreeDays = result.AllClients + (int)(result.AllClients * 0.7);

            result.AllDreamUsers = helper.getServiceProviders(CoreController.UserWorkCode.Dream.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllRouqiaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Rouqia.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllIftaaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Iftaa.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllIstasharaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Istishara.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllMedicalUsers = helper.getServiceProviders(CoreController.UserWorkCode.Medical.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllLawUsers = helper.getServiceProviders(CoreController.UserWorkCode.Law.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.CreationDate = DateTime.Now;
            result.LastModificationDate = DateTime.Now;
            result.status = "Active";
            Statistics current = db.Statistics.Where(a => a.status.Equals("Active")).FirstOrDefault();
            if (current != null)
            {
                current.status = "Not_Active";
                db.Entry(current).State = System.Data.Entity.EntityState.Modified;
            }
            
            db.Statistics.Add(result);
            
            db.SaveChanges();
            return Json("200", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ServiceProvider(String type)
        {
            return View();
        }
    }
}
