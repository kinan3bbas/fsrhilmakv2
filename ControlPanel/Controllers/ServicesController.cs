using ControlPanel.Extra;
using ControlPanel.Extras;
using ControlPanel.Models;
using ControlPanel.ViewModels;
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
    public class ServicesController : Controller
    {
        private UserHelperLibrary helper = new UserHelperLibrary();



        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: services
        public ActionResult Index()
        {
            var services = db.Services.Include("Comments")
                .Include("ServicePath")
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator")
                .ToList();
            List<ServiceViewModel> result = new List<ServiceViewModel>();
            foreach (var item in services)
            {
                result.Add(getMapping(item));
            }
            return View(result);
        }

      

        // GET: UserWorks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserWork userWork = db.UserWorks.Find(id);
            if (userWork == null)
            {
                return HttpNotFound();
            }
            return View(userWork);
        }

        // POST: UserWorks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Name,AdjectiveName,Enabled")] UserWork userWork)
        {
            if (ModelState.IsValid)
            {
                UserWork origin = db.UserWorks.Where(y => y.id == userWork.id).First();
                origin.Name = userWork.Name;
                origin.AdjectiveName = userWork.AdjectiveName;
                origin.Enabled = userWork.Enabled;
                origin.LastModificationDate = DateTime.Now;
                db.Entry(origin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userWork);
        }

        // GET: UserWorks/Delete/5
     



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




            return result;
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
