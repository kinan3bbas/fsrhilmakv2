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
    builder.EntitySet<Competition>("Competitions");
    builder.EntitySet<ApplicationUser>("ApplicationUsers"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */

        [AllowAnonymous]
    public class CompetitionsController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();

        // GET: odata/Competitions
        [EnableQuery]
        public IQueryable<Competition> GetCompetitions()
        {
            return db.Competitions;
        }

        // GET: odata/Competitions(5)
        [EnableQuery]
        public SingleResult<Competition> GetCompetition([FromODataUri] int key)
        {
            return SingleResult.Create(db.Competitions.Where(Competition => Competition.id == key));
        }

        // PUT: odata/Competitions(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Competition> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Competition Competition = db.Competitions.Find(key);
            if (Competition == null)
            {
                return NotFound();
            }

            patch.Put(Competition);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetitionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(Competition);
        }

        // POST: odata/Competitions
        public IHttpActionResult Post(Competition Competition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Competition.CreationDate = DateTime.Now;
            Competition.LastModificationDate = DateTime.Now;
            //Competition.Creator = core.getCurrentUser();
            //Competition.Modifier = core.getCurrentUser();
            db.Competitions.Add(Competition);
            db.SaveChanges();

            return Created(Competition);
        }

        // PATCH: odata/Competitions(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Competition> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Competition Competition = db.Competitions.Find(key);
            if (Competition == null)
            {
                return NotFound();
            }

            patch.Patch(Competition);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetitionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(Competition);
        }

        // DELETE: odata/Competitions(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Competition Competition = db.Competitions.Find(key);
            if (Competition == null)
            {
                return NotFound();
            }

            db.Competitions.Remove(Competition);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        //// GET: odata/Competitions(5)/Creator
        //[EnableQuery]
        //public SingleResult<ApplicationUser> GetCreator([FromODataUri] int key)
        //{
        //    return SingleResult.Create(db.Competitions.Where(m => m.id == key).Select(m => m.Creator));
        //}

        //// GET: odata/Competitions(5)/Modifier
        //[EnableQuery]
        //public SingleResult<ApplicationUser> GetModifier([FromODataUri] int key)
        //{
        //    return SingleResult.Create(db.Competitions.Where(m => m.id == key).Select(m => m.Modifier));
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CompetitionExists(int key)
        {
            return db.Competitions.Count(e => e.id == key) > 0;
        }
    }
}
