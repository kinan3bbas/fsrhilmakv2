using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using fsrhilmakv2.Models;
using fsrhilmakv2.ViewModels;

namespace fsrhilmakv2.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using fsrhilmakv2.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<SystemParameter>("SystemParameters");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [Authorize]
    public class ServicesController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        CoreController core = new CoreController();

        [AllowAnonymous]
        // GET: odata/Services
        [EnableQuery]
        public IQueryable<Service> GetServices()
        {
            return db.Services;
        }

        [AllowAnonymous]
        // GET: odata/Services(5)
        [EnableQuery]
        public SingleResult<Service> GetService([FromODataUri] int key)
        {
            return SingleResult.Create(db.Services.Where(Service => Service.id == key));
        }

        // PUT: odata/Services(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Service> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Service Service = db.Services.Find(key);
            if (Service == null)
            {
                return NotFound();
            }

            patch.Put(Service);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(Service);
        }

        // POST: odata/Services
        public IHttpActionResult Post(Service Service)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Get Service Path
            if (!Service.ServicePathId.Equals(null))
            {
                ServicePath path = db.ServicePaths.Find(Service.ServicePathId);
                if (path == null)
                    core.throwExcetpion("No Matching Service Path");
                Service.ServicePath = path;
            }

            //Get Service user work
            if (!Service.UserWorkId.Equals(null))
            {
                UserWork work = db.UserWorks.Find(Service.UserWorkId);
                if (work == null)
                    core.throwExcetpion("No Matching UserWork!");
                Service.UserWork = work;

            }

            //Get Service Provider
            if (!Service.ServiceProviderId.Equals(null))
            {
                ApplicationUser serviceProvider = db.Users.Find(Service.ServiceProviderId);
                if (serviceProvider == null)
                    core.throwExcetpion("No Matching Service Provider");
                Service.ServiceProvider = serviceProvider;
            }

            //Check Private Service Price
            if (Service.PrivateService)
            {
                if (Service.PrivateServicePrice.Equals(null))
                    core.throwExcetpion("Private service price can't be null!");
            }
            ApplicationUser currentUser = core.getCurrentUser();
            Service.Status = CoreController.ServiceStatus.Active.ToString();
            Service.CreationDate = DateTime.Now;
            Service.LastModificationDate = DateTime.Now;
            Service.CreatorId = core.getCurrentUser().Id;
            Service.ModifierId = core.getCurrentUser().Id;
            //Service.Creator = core.getCurrentUser();
            //Service.Modifier = core.getCurrentUser();
            
            db.Services.Add(Service);
            addComment(Service.id, Service.ServiceProviderId, Service.ServicePathId);
            db.SaveChanges();

            return Created(Service);
        }

        private void addComment(int serviceId,string serviceProviderId, int servicePathId)
        {
            ServiceComment comment = new ServiceComment();
            comment.CreatorId = serviceProviderId;
            comment.ServiceId = serviceId;
            comment.CreationDate = DateTime.Now;
            comment.LastModificationDate = DateTime.Now;
            comment.CreatorName = db.Users.Find(serviceProviderId).Name;
            comment.Text = db.ServicePaths.Find(servicePathId).Message != null ? db.ServicePaths.Find(servicePathId).Message : "";
            db.ServiceComments.Add(comment);
        }

        // PATCH: odata/Services(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Service> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Service Service = db.Services.Where(a=>a.id.Equals(key))
                .Include("ServiceProvider")
                .Include("ServicePath")
                .Include("Comments")
                .Include("Creator")
                .Include("Modifier")
                .Include("UserWork")
                .FirstOrDefault();
            if (Service == null)
            {
                return NotFound();
            }
            bool ServiceExplained = false;
            bool Rating = false;
            foreach (var fieldname in patch.GetChangedPropertyNames())
            {
                if (fieldname.Equals("Explanation"))
                {
                    ServiceExplained = true;

                }
                if (fieldname.Equals("RatingDate"))
                {
                     Rating= true;

                }
            }

            patch.Patch(Service);

            try
            {
                if (ServiceExplained)
                {
                    Service.ExplanationDate = DateTime.Now;
                    Service.Status = CoreController.ServiceStatus.Done.ToString();
                }
                if (Rating)
                {
                    if (!Service.Status.Equals("Done"))
                        core.throwExcetpion("User can't rate interpreter until he explain the dream!");
                    if (Service.UserRating < 0 || Service.UserRating > 5)
                    {
                        core.throwExcetpion("Rating can only be between 0 and 5");
                    }
                    Service.UserRating = Service.UserRating;
                    Service.RatingDate = DateTime.Now;
                }
               
                Service.LastModificationDate = DateTime.Now;
                //Service.Modifier = core.getCurrentUser();
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(Service);
        }

        // DELETE: odata/Services(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Service Service = db.Services.Find(key);
            if (Service == null)
            {
                return NotFound();
            }

            db.Services.Remove(Service);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ServiceExists(int key)
        {
            return db.Services.Count(e => e.id == key) > 0;
        }


        //public ServiceViewModel getMapping(Service service)
        //{
        //    AccountController accountCont = new AccountController();
        //    ServiceViewModel result = new ServiceViewModel();
        //    result.Comments = service.Comments;
        //    result.Country = service.Country;
        //    result.CreationDate = service.CreationDate;
        //    result.Creator = service.Creator;
        //    result.Description = service.Description;
        //    result.CreatorId = service.CreatorId;
        //    result.DidYouExorcism = service.DidYouExorcism;
        //    result.DreamDate = service.DreamDate;
        //    result.Explanation = service.Explanation;
        //    result.ExplanationDate = service.ExplanationDate;
        //    result.HaveYouPrayedBeforeTheDream = service.HaveYouPrayedBeforeTheDream;
        //    result.Id = service.id;
        //    result.IsThereWakefulness = service.IsThereWakefulness;
        //    result.JobStatus = service.JobStatus;
        //    result.KidsStatus = service.KidsStatus;
        //    result.LastModificationDate = service.LastModificationDate;
        //    result.Modifier = service.Modifier;
        //    result.ModifierId = service.ModifierId;
        //    result.Name = service.Name;
        //    result.numberOfLikes = service.numberOfLikes;
        //    result.numberOfViews = service.numberOfViews;
        //    result.PrivateService = service.PrivateService;
        //    result.PrivateServicePrice = service.PrivateServicePrice;
        //    result.PublicServiceAction = service.PublicServiceAction;
        //    result.RegligionStatus = service.RegligionStatus;
        //    result.ServicePathId = service.ServicePathId;
        //    result.ServiceProvider = service.ServiceProvider;
        //    result.ServiceProviderId = service.ServiceProviderId;
        //    result.Sex = service.Sex;
        //    result.SocialStatus = service.SocialStatus;
        //    result.Status = service.Status;
        //    result.UserWork = service.UserWork;
        //    result.UserWorkId = service.UserWorkId;
        //    result.ServicePath = accountCont.GetServicePathForProvider(service.ServiceProviderId, service.ServicePathId);

        //    return result;
        //}
    }
}
