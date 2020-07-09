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
    public class ServicesController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: odata/Services
        [EnableQuery]
        public IQueryable<Service> GetServices()
        {
            return db.Services;
        }

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

            Service.CreationDate = DateTime.Now;
            Service.LastModificationDate = DateTime.Now;
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
