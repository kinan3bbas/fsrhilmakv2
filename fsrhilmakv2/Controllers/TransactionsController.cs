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
    builder.EntitySet<Transaction>("Transactions");
    builder.EntitySet<ApplicationUser>("ApplicationUsers"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */

        [AllowAnonymous]
    public class TransactionsController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();

        // GET: odata/Transactions
        [EnableQuery]
        public IQueryable<Transaction> GetTransactions()
        {
            return db.Transactions;
        }

        // GET: odata/Transactions(5)
        [EnableQuery]
        public SingleResult<Transaction> GetTransaction([FromODataUri] int key)
        {
            return SingleResult.Create(db.Transactions.Where(Transaction => Transaction.id == key));
        }

        // PUT: odata/Transactions(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Transaction> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Transaction Transaction = db.Transactions.Find(key);
            if (Transaction == null)
            {
                return NotFound();
            }

            patch.Put(Transaction);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(Transaction);
        }

        // POST: odata/Transactions
        public IHttpActionResult Post(Transaction Transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Transaction.CreationDate = DateTime.Now;
            Transaction.LastModificationDate = DateTime.Now;
            //Transaction.Creator = core.getCurrentUser();
            //Transaction.Modifier = core.getCurrentUser();
            db.Transactions.Add(Transaction);
            db.SaveChanges();

            return Created(Transaction);
        }

        // PATCH: odata/Transactions(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Transaction> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Transaction Transaction = db.Transactions.Find(key);
            if (Transaction == null)
            {
                return NotFound();
            }

            patch.Patch(Transaction);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(Transaction);
        }

        // DELETE: odata/Transactions(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Transaction Transaction = db.Transactions.Find(key);
            if (Transaction == null)
            {
                return NotFound();
            }

            db.Transactions.Remove(Transaction);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        //// GET: odata/Transactions(5)/Creator
        //[EnableQuery]
        //public SingleResult<ApplicationUser> GetCreator([FromODataUri] int key)
        //{
        //    return SingleResult.Create(db.Transactions.Where(m => m.id == key).Select(m => m.Creator));
        //}

        //// GET: odata/Transactions(5)/Modifier
        //[EnableQuery]
        //public SingleResult<ApplicationUser> GetModifier([FromODataUri] int key)
        //{
        //    return SingleResult.Create(db.Transactions.Where(m => m.id == key).Select(m => m.Modifier));
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionExists(int key)
        {
            return db.Transactions.Count(e => e.id == key) > 0;
        }
    }
}
