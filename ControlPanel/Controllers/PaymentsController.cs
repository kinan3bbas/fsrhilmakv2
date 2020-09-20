
using ControlPanel.Extra;
using ControlPanel.Extras;
using ControlPanel.Models;
using ControlPanel.ViewModels;
using fsrhilmakv2.Extra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;



namespace ControlPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentsController : Controller
    {
        private UserHelperLibrary helper = new UserHelperLibrary();




        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? UserWorkId, string status, String fromDate = "", String toDate = "")

        {
            DateTime from = new DateTime(2000, 1, 1);
            DateTime to = new DateTime(3000, 1, 1);
            if (!fromDate.Equals("") && fromDate != null)
            {
                DateTime.TryParse(fromDate, out from);
            }
            if (!toDate.Equals("") && toDate != null)
            {
                DateTime.TryParse(toDate, out to);
            }
            List<Payment> payments = db.Payments
                .Include("Service")
                .Include("Service.ServiceProvider")
                .Include("Service.ServicePath")
                .Include("Creator")
                .ToList();
            payments=payments.Where(a => a.CreationDate.CompareTo(from) >= 0 && a.CreationDate.CompareTo(to) <= 0).ToList();
            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled), "id", "AdjectiveName");
            return View(payments);
        }



        // GET: serviceINFO
        public ActionResult ServiceInfo(int id)
        {
           var services = db.Services.Where(a => a.id.Equals(id))
                .Include("Comments")
                .Include("ServicePath")
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator").FirstOrDefault(); ;

           
           
            ViewBag.userId = id;
            return View(getMapping(services));

        }

        public ActionResult Edit(int id)
        {
            var services = db.Services.Where(a => a.id.Equals(id))
                 .Include("Comments")
                 .Include("ServicePath")
                 .Include("UserWork")
                 .Include("ServiceProvider")
                 .Include("Creator").FirstOrDefault(); ;



            ViewBag.userId = id;
            return View(getMapping(services));

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServiceViewModel service)
        {
            if (service == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                Service temp = db.Services.Where(a => a.id.Equals(service.id)).FirstOrDefault();
                temp.id = service.id;
                temp.Name = service.Name;
                temp.Status = service.Status;
                temp.Sex = service.Sex;
                temp.UserWork.Name = service.UserWork.Name;
                temp.ServicePath = service.ServicePath;
                temp.Creator.Name = service.Creator.Name;
                temp.KidsStatus = service.KidsStatus;
                temp.IsThereWakefulness = service.IsThereWakefulness;
                temp.Country = service.Country;
                temp.HaveYouPrayedBeforeTheDream = service.HaveYouPrayedBeforeTheDream;
                temp.DidYouExorcism = service.DidYouExorcism;
                temp.RegligionStatus = service.RegligionStatus;
                temp.SocialStatus = service.SocialStatus;
                temp.JobStatus = service.JobStatus;
                temp.Country = service.Country;
                db.Entry(temp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = service.id });

            }
            return View(service)
;
        }

        public  ActionResult InterpreterServices(String id, String fromDate = "", String toDate = "")
        {

            DateTime from = new DateTime(2000, 1, 1);
            DateTime to = new DateTime(3000, 1, 1);
            if (!fromDate.Equals("") && fromDate != null)
            {
                DateTime.TryParse(fromDate, out from);
            }
            if (!toDate.Equals("") && toDate != null)
            {
                DateTime.TryParse(toDate, out to);
            }
            ApplicationUser user = db.Users.Find(id);
            List<Service> services = new List<Service>();
            if (user.Type.Equals("Service_Provider"))
                services = db.Services.Where(a => a.ServiceProviderId.Equals(id) && a.Status == "Done").OrderByDescending(a => a.CreationDate).ToList();
            services = services.Where(a => a.CreationDate.CompareTo(from) >= 0 && a.CreationDate.CompareTo(to) <= 0).ToList();

            List<ServiceViewModel> result = new List<ServiceViewModel>();
            foreach (var item in services)
            {
                result.Add(getMapping(item));
            }
            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled), "id", "AdjectiveName");
            ViewBag.Type = user.Type;
            return View(result);
        }



        public  ActionResult ServicesUnderInterpretation(String id, String fromDate = "", String toDate = "")
        {

            DateTime from = new DateTime(2000, 1, 1);
            DateTime to = new DateTime(3000, 1, 1);
            if (!fromDate.Equals("") && fromDate != null)
            {
                DateTime.TryParse(fromDate, out from);
            }
            if (!toDate.Equals("") && toDate != null)
            {
                DateTime.TryParse(toDate, out to);
            }
            ApplicationUser user = db.Users.Find(id);
            List<Service> services = new List<Service>();
            if (user.Type.Equals("Client"))
                services = db.Services.Where(a => a.CreatorId.Equals(id) && a.Status == "Active").OrderByDescending(a => a.CreationDate).ToList();
            services = services.Where(a => a.CreationDate.CompareTo(from) >= 0 && a.CreationDate.CompareTo(to) <= 0).ToList();

            List<ServiceViewModel> result = new List<ServiceViewModel>();
            foreach (var item in services)
            {
                result.Add(getMapping(item));
            }
            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled), "id", "AdjectiveName");
            ViewBag.Type = user.Type;
            return View(result);
        }


        public ServiceViewModel getMapping(Service service)
        {
            List<Service> allService = db.Services.Where(a => a.ServiceProviderId.Equals(service.ServiceProviderId)
                && a.ServicePathId.Equals(service.ServicePathId)
                && a.Status.Equals(CoreController.ServiceStatus.Active.ToString())).ToList();

            List<Service> services = helper.getUserServices(service.ServiceProviderId);
            List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
            List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
            double speed = UserHelperLibrary.ServiceProviderSpeed(helper.findUser(service.ServiceProviderId), doneServices.Count);

            AccountController accountCont = new AccountController();
            ServiceViewModel result = new ServiceViewModel();
            result.Comments = service.Comments;
            result.Country = service.Country;
            result.CreationDate = service.CreationDate;
            result.Creator = service.Creator;
            result.Description = service.Description;
            result.CreatorId = service.CreatorId;
            result.DidYouExorcism = service.DidYouExorcism;
            result.DreamDate = service.DreamDate;
            result.Explanation = service.Explanation;
            result.ExplanationDate = service.ExplanationDate;
            result.HaveYouPrayedBeforeTheDream = service.HaveYouPrayedBeforeTheDream;
            result.id = service.id;
            result.IsThereWakefulness = service.IsThereWakefulness;
            result.JobStatus = service.JobStatus;
            result.KidsStatus = service.KidsStatus;
            result.LastModificationDate = service.LastModificationDate;
            result.Modifier = service.Modifier;
            result.ModifierId = service.ModifierId;
            result.Name = service.Name;
            result.numberOfLikes = service.numberOfLikes;
            result.numberOfViews = service.numberOfViews;
            result.PrivateService = service.PrivateService;
            result.PrivateServicePrice = service.PrivateServicePrice;
            result.PublicServiceAction = service.PublicServiceAction;
            result.RegligionStatus = service.RegligionStatus;
            result.ServicePathId = service.ServicePathId;
            result.ServiceProvider = service.ServiceProvider;
            result.ServiceProviderId = service.ServiceProviderId;
            result.Sex = service.Sex;
            result.SocialStatus = service.SocialStatus;
            result.Status = service.Status;
            result.UserWork = service.UserWork;
            result.UserWorkId = service.UserWorkId;
            result.ServicePath = service.ServicePath;
            result.NumberOfAllPeopleWaiting = allService.Count > 0 ? allService.Count : 0;
            result.NumberOfRemainingPeople = allService.Count > 0 ? allService.Where(a => a.CreationDate.CompareTo(service.CreationDate) < 0).Count() : 0;
            result.AvgWaitingTime = UserHelperLibrary.getWaitingTimeMessage(Double.Parse(speed.ToString()),
                Double.Parse(result.NumberOfRemainingPeople.ToString())).Replace("Your average waiting time is ", "");




            return result;
        }
       

        // GET: Payments/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Payments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Payments/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Payments/Edit/5
      

    }
}
