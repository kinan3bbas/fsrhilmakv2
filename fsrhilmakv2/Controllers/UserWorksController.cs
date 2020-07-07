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
    builder.EntitySet<UserWork>("UserWorks");
    builder.EntitySet<ApplicationUser>("ApplicationUsers"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class UserWorksController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();

        // GET: odata/UserWorks
        [EnableQuery]
        public IQueryable<UserWork> GetUserWorks()
        {
            return db.UserWorks;
        }

        // GET: odata/UserWorks(5)
        [EnableQuery]
        public SingleResult<UserWork> GetUserWork([FromODataUri] int key)
        {
            return SingleResult.Create(db.UserWorks.Where(userWork => userWork.id == key));
        }

        // PUT: odata/UserWorks(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<UserWork> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserWork userWork = db.UserWorks.Find(key);
            if (userWork == null)
            {
                return NotFound();
            }

            patch.Put(userWork);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserWorkExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(userWork);
        }

        // POST: odata/UserWorks
        public IHttpActionResult Post(UserWork userWork)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userWork.CreationDate = DateTime.Now;
            userWork.LastModificationDate = DateTime.Now;
            userWork.Creator = core.getCurrentUser();
            userWork.Modifier = core.getCurrentUser();
            db.UserWorks.Add(userWork);
            db.SaveChanges();

            return Created(userWork);
        }

        // PATCH: odata/UserWorks(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<UserWork> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserWork userWork = db.UserWorks.Find(key);
            if (userWork == null)
            {
                return NotFound();
            }

            patch.Patch(userWork);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserWorkExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(userWork);
        }

        // DELETE: odata/UserWorks(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            UserWork userWork = db.UserWorks.Find(key);
            if (userWork == null)
            {
                return NotFound();
            }

            db.UserWorks.Remove(userWork);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/UserWorks(5)/Creator
        [EnableQuery]
        public SingleResult<ApplicationUser> GetCreator([FromODataUri] int key)
        {
            return SingleResult.Create(db.UserWorks.Where(m => m.id == key).Select(m => m.Creator));
        }

        // GET: odata/UserWorks(5)/Modifier
        [EnableQuery]
        public SingleResult<ApplicationUser> GetModifier([FromODataUri] int key)
        {
            return SingleResult.Create(db.UserWorks.Where(m => m.id == key).Select(m => m.Modifier));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserWorkExists(int key)
        {
            return db.UserWorks.Count(e => e.id == key) > 0;
        }
    }
}
