using ControlPanel.Extra;
using ControlPanel.Extras;
using ControlPanel.Models;
using ControlPanel.ViewModels;
using fsrhilmakv2.Extra;
using PagedList;
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
    public class ServicesController : Controller
    {
        private UserHelperLibrary helper = new UserHelperLibrary();



        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: services
        public ActionResult Index(int? UserWorkId, int? page, string status, String fromDate = "", String toDate = "")
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
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


            List<Service> services = db.Services.Where(a => a.CreationDate.CompareTo(from) >= 0 && a.CreationDate.CompareTo(to) <= 0
            &&((status != null && !status.Equals(""))? a.Status.Equals(status): a.Status.Equals("Active"))
            ).OrderByDescending(a => a.CreationDate)
                .Include("ServicePath")
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator")
                .ToList();
            //if (status != null && !status.Equals(""))
            //    services = services.Where(a => a.Status.Equals(status)).ToList();
            //else
            //    services = services.Where(a => a.Status.Equals("Active")).ToList();

            if (UserWorkId != null)
            {

                services = services.Where(a => a.UserWorkId .Equals( UserWorkId)).ToList();
                //services = bindings.Select(a => a.U).ToList();
            }

            //List<ServiceViewModel> result = new List<ServiceViewModel>();
            //foreach (var item in Pservices)
            //{
            //    result.Add(getMappingv2(item));
            //}

            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled), "id", "AdjectiveName");
            ViewBag.numberOfServices = services.Count();
            return View(services.ToPagedList(pageNumber,pageSize));
        }

        public ActionResult Index2(int? UserWorkId, int? size, int? page, string status, String fromDate = "", String toDate = "")
        {
            int pageSize = (size ?? 100);
            int pageNumber = (page ?? 1);
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


            List<Service> services = db.Services.Where(a => a.CreationDate.CompareTo(from) >= 0 && a.CreationDate.CompareTo(to) <= 0
            && ((status != null && !status.Equals("")) ? a.Status.Equals(status) : a.Status.Equals("Active"))
            ).OrderByDescending(a => a.CreationDate)
                .Include("ServicePath")
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator")
                .ToList();
            //if (status != null && !status.Equals(""))
            //    services = services.Where(a => a.Status.Equals(status)).ToList();
            //else
            //    services = services.Where(a => a.Status.Equals("Active")).ToList();

            if (UserWorkId != null)
            {

                services = services.Where(a => a.UserWorkId.Equals(UserWorkId)).ToList();
                //services = bindings.Select(a => a.U).ToList();
            }

            //List<ServiceViewModel> result = new List<ServiceViewModel>();
            //foreach (var item in Pservices)
            //{
            //    result.Add(getMappingv2(item));
            //}

            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled), "id", "AdjectiveName");
            ViewBag.numberOfServices = services.Count();
            return View(services.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult PublicServices(int? UserWorkId, int? page, string status, String fromDate = "", String toDate = "")
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
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

            List<PublicService> publicServices = db.PublicServices
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator").ToList();
            if (status != null && !status.Equals(""))
                publicServices = publicServices.Where(a => a.Status.Equals(status)).OrderByDescending(r => r.CreationDate).ToList();

            if (UserWorkId != null)
            {

                publicServices = publicServices.Where(a => a.UserWorkId.Equals(UserWorkId)).OrderByDescending(r => r.CreationDate).ToList();
            }
            publicServices = publicServices.Where(a => a.CreationDate.CompareTo(from) >= 0 && a.CreationDate.CompareTo(to) <= 0).ToList();
            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled), "id", "AdjectiveName");
            ViewBag.numberOfServices = publicServices.Count();
            return View(publicServices.ToPagedList(pageNumber, pageSize));

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

        //public ActionResult Edit(int id)
        //{
        //    var services = db.Services.Where(a => a.id.Equals(id))
        //         .Include("Comments")
        //         .Include("ServicePath")
        //         .Include("UserWork")
        //         .Include("ServiceProvider")
        //         .Include("Creator").FirstOrDefault(); ;



        //    ViewBag.userId = id;
        //    return View(getMapping(services));

        //}
        public ActionResult Edit(int id)
        {
          
            Service services = db.Services.Find(id); ;
            if (services == null)
            {
                return HttpNotFound();
            }
            ViewBag.ServiceProviderId = new SelectList(db.Users.Where(a=>a.Type.Equals(CoreController.UserType.Service_Provider.ToString())&&
            a.Status.Equals("Active")), "Id", "Name");

            return View(services);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Service service)
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
                temp.Sex = service.Sex;
                temp.KidsStatus = service.KidsStatus;
                temp.IsThereWakefulness = service.IsThereWakefulness;
                temp.Country = service.Country;
                temp.HaveYouPrayedBeforeTheDream = service.HaveYouPrayedBeforeTheDream;
                temp.DidYouExorcism = service.DidYouExorcism;
                temp.RegligionStatus = service.RegligionStatus;
                temp.SocialStatus = service.SocialStatus;
                temp.JobStatus = service.JobStatus;
                temp.ServiceProviderId = service.ServiceProviderId;
                temp.Country = service.Country;
                db.Entry(temp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(service)
;
        }

        public  ActionResult InterpreterServices(String userId, String fromDate = "", String toDate = "")
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
            ApplicationUser user = db.Users.Find(userId);
            List<Service> services = db.Services.Include("Comments")
                .Include("ServicePath")
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator")
                .ToList();
            if (user.Type.Equals("Service_Provider"))
                services = services.Where(a => a.ServiceProviderId.Equals(userId) && a.Status == "Done").OrderByDescending(a => a.CreationDate).ToList();
            else
            {
                services = services.Where(a => a.CreatorId.Equals(userId) && a.Status == "Done").OrderByDescending(a => a.CreationDate).ToList();
            }
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



        public  ActionResult ServicesUnderInterpretation(String userId, String fromDate = "", String toDate = "")
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
            ApplicationUser user = db.Users.Find(userId);
            List<Service> services = db.Services.Include("Comments")
                .Include("ServicePath")
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator")
                .ToList();
            if (user.Type.Equals(CoreController.UserType.Service_Provider.ToString())) {
                services = db.Services.Where(a => a.ServiceProviderId.Equals(userId) && a.Status == "Active").OrderByDescending(a => a.CreationDate).ToList();
                
            }
                
            else
            {
                services = db.Services.Where(a => a.CreatorId.Equals(userId) && a.Status == "Active").OrderByDescending(a => a.CreationDate).ToList();
            }
            services = services.Where(a => a.CreationDate.CompareTo(from) >= 0 && a.CreationDate.CompareTo(to) <= 0).ToList();
            List<ServiceViewModel> result = new List<ServiceViewModel>();
            foreach (var item in services)
            {
                result.Add(getMappingv2(item));
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
            speed = speed < 1 ? 1 : speed;
            double avg = UserHelperLibrary.ServiceProviderAvgServices(helper.findUser(service.ServiceProviderId), services.Count);
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
            result.Id = service.id;
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
            result.ServiceProviderNewDate = service.ServiceProviderNewDate;

            result.ServiceProviderSpeed = speed;
            result.ServiceProviderAvgServices = avg == 0 ? 1 : avg;

            return result;


        }


        public ServiceViewModel getMappingv2(Service service)
        {
            List<Service> allService = db.Services.Where(a => a.ServiceProviderId.Equals(service.ServiceProviderId)
                 && a.ServicePathId.Equals(service.ServicePathId)
                 && a.Status.Equals(CoreController.ServiceStatus.Active.ToString())).ToList();

            List<Service> services = helper.getUserServices(service.ServiceProviderId);
            List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
            List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
            double speed = UserHelperLibrary.ServiceProviderSpeed(helper.findUser(service.ServiceProviderId), doneServices.Count);
            speed = speed < 1 ? 1 : speed;
            double avg = UserHelperLibrary.ServiceProviderAvgServices(helper.findUser(service.ServiceProviderId), services.Count);
            AccountController accountCont = new AccountController();
            ServiceViewModel result = new ServiceViewModel();
            result.Country = service.Country;
            result.CreationDate = service.CreationDate;
            result.Creator = service.Creator;
            result.Description = service.Description;
            result.CreatorId = service.CreatorId;
            result.Id = service.id;
            result.Name = service.Name;
            result.numberOfLikes = service.numberOfLikes;
            result.numberOfViews = service.numberOfViews;
            result.PrivateService = service.PrivateService;
            result.PrivateServicePrice = service.PrivateServicePrice;
            result.PublicServiceAction = service.PublicServiceAction;
            result.ServicePathId = service.ServicePathId;
            result.ServiceProvider = service.ServiceProvider;
            result.ServiceProviderId = service.ServiceProviderId;
            result.Status = service.Status;
            result.UserWork = service.UserWork;
            result.UserWorkId = service.UserWorkId;
            result.ServicePath = service.ServicePath;
            result.ServiceProviderNewDate = service.ServiceProviderNewDate;

            result.ServiceProviderSpeed = speed;
            result.ServiceProviderAvgServices = avg == 0 ? 1 : avg;

            return result;


        }


        // GET: Services/Delete/5
        public ActionResult Delete(int id)
        {
            
            Service services = db.Services.Find(id);
            if (services == null)
            {
                return HttpNotFound();
            }
            return View(services);
        }




        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Service services = db.Services.Find(id);
            db.Services.Remove(services);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
