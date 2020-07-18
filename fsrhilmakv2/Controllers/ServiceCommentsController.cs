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
    config.Routes.MapODataServiceCommentRoute("odata", "odata", builder.GetEdmModel());
    */
    [Authorize]
    public class ServiceCommentsController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        CoreController core = new CoreController();

        // GET: odata/ServiceComments
        [EnableQuery]
        public IQueryable<ServiceComment> GetServiceComments()
        {
            return db.ServiceComments;
        }

        // GET: odata/ServiceComments(5)
        [EnableQuery]
        public SingleResult<ServiceComment> GetServiceComment([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceComments.Where(ServiceComment => ServiceComment.id == key));
        }

        // PUT: odata/ServiceComments(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<ServiceComment> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceComment ServiceComment = db.ServiceComments.Find(key);
            if (ServiceComment == null)
            {
                return NotFound();
            }

            patch.Put(ServiceComment);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceCommentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(ServiceComment);
        }

        // POST: odata/ServiceComments
        public IHttpActionResult Post(ServiceComment ServiceComment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            ApplicationUser currentUser = core.getCurrentUser();
            ServiceComment.CreationDate = DateTime.Now;
            ServiceComment.LastModificationDate = DateTime.Now;
            ServiceComment.CreatorId = core.getCurrentUser().Id;
            ServiceComment.ModifierId = core.getCurrentUser().Id;

            
            db.ServiceComments.Add(ServiceComment);
            db.SaveChanges();

            return Created(ServiceComment);
        }

        // PATCH: odata/ServiceComments(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<ServiceComment> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceComment ServiceComment = db.ServiceComments.Find(key);
            if (ServiceComment == null)
            {
                return NotFound();
            }

            patch.Patch(ServiceComment);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceCommentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(ServiceComment);
        }

        // DELETE: odata/ServiceComments(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            ServiceComment ServiceComment = db.ServiceComments.Find(key);
            if (ServiceComment == null)
            {
                return NotFound();
            }

            db.ServiceComments.Remove(ServiceComment);
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

        private bool ServiceCommentExists(int key)
        {
            return db.ServiceComments.Count(e => e.id == key) > 0;
        }
    }
}
