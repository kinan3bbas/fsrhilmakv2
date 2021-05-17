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
    builder.EntitySet<Quran>("Qurans");
    builder.EntitySet<ApplicationUser>("ApplicationUsers"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */

        [AllowAnonymous]
    public class QuransController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();

        // GET: odata/Qurans
        [EnableQuery (AllowedQueryOptions =System.Web.Http.OData.Query.AllowedQueryOptions.All)]
        public IQueryable<Quran> GetQurans()
        {
            return db.Qurans;
        }

        // GET: odata/Qurans(5)
        [EnableQuery]
        public SingleResult<Quran> GetQuran([FromODataUri] int key)
        {
            return SingleResult.Create(db.Qurans.Where(Quran => Quran.id == key));
        }

        // PUT: odata/Qurans(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Quran> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Quran Quran = db.Qurans.Find(key);
            if (Quran == null)
            {
                return NotFound();
            }

            patch.Put(Quran);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuranExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(Quran);
        }

        // POST: odata/Qurans
        public IHttpActionResult Post(Quran Quran)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Quran.CreationDate = DateTime.Now;
            Quran.LastModificationDate = DateTime.Now;
            //Quran.Creator = core.getCurrentUser();
            //Quran.Modifier = core.getCurrentUser();
            db.Qurans.Add(Quran);
            db.SaveChanges();

            return Created(Quran);
        }

        // PATCH: odata/Qurans(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Quran> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Quran Quran = db.Qurans.Find(key);
            if (Quran == null)
            {
                return NotFound();
            }

            patch.Patch(Quran);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuranExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(Quran);
        }

        // DELETE: odata/Qurans(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Quran Quran = db.Qurans.Find(key);
            if (Quran == null)
            {
                return NotFound();
            }

            db.Qurans.Remove(Quran);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        //// GET: odata/Qurans(5)/Creator
        //[EnableQuery]
        //public SingleResult<ApplicationUser> GetCreator([FromODataUri] int key)
        //{
        //    return SingleResult.Create(db.Qurans.Where(m => m.id == key).Select(m => m.Creator));
        //}

        //// GET: odata/Qurans(5)/Modifier
        //[EnableQuery]
        //public SingleResult<ApplicationUser> GetModifier([FromODataUri] int key)
        //{
        //    return SingleResult.Create(db.Qurans.Where(m => m.id == key).Select(m => m.Modifier));
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuranExists(int key)
        {
            return db.Qurans.Count(e => e.id == key) > 0;
        }
    }
}
