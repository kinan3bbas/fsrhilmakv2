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
            db.SaveChanges();

            return Created(Service);
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

            Service Service = db.Services.Find(key);
            if (Service == null)
            {
                return NotFound();
            }

            patch.Patch(Service);

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
    }
}
