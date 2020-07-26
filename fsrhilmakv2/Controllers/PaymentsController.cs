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
    builder.EntitySet<Payment>("Payments");
    builder.EntitySet<ApplicationUser>("ApplicationUsers"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */

        [AllowAnonymous]
    public class PaymentsController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();

        // GET: odata/Payments
        [EnableQuery]
        public IQueryable<Payment> GetPayments()
        {
            return db.Payments;
        }

        // GET: odata/Payments(5)
        [EnableQuery]
        public SingleResult<Payment> GetPayment([FromODataUri] int key)
        {
            return SingleResult.Create(db.Payments.Where(Payment => Payment.id == key));
        }

        // PUT: odata/Payments(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Payment> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Payment Payment = db.Payments.Find(key);
            if (Payment == null)
            {
                return NotFound();
            }

            patch.Put(Payment);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(Payment);
        }

        // POST: odata/Payments
        public IHttpActionResult Post(Payment Payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Payment.CreationDate = DateTime.Now;
            Payment.LastModificationDate = DateTime.Now;
            //Payment.Creator = core.getCurrentUser();
            //Payment.Modifier = core.getCurrentUser();
            db.Payments.Add(Payment);
            db.SaveChanges();

            return Created(Payment);
        }

        // PATCH: odata/Payments(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Payment> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Payment Payment = db.Payments.Find(key);
            if (Payment == null)
            {
                return NotFound();
            }

            patch.Patch(Payment);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(Payment);
        }

        // DELETE: odata/Payments(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Payment Payment = db.Payments.Find(key);
            if (Payment == null)
            {
                return NotFound();
            }

            db.Payments.Remove(Payment);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        //// GET: odata/Payments(5)/Creator
        //[EnableQuery]
        //public SingleResult<ApplicationUser> GetCreator([FromODataUri] int key)
        //{
        //    return SingleResult.Create(db.Payments.Where(m => m.id == key).Select(m => m.Creator));
        //}

        //// GET: odata/Payments(5)/Modifier
        //[EnableQuery]
        //public SingleResult<ApplicationUser> GetModifier([FromODataUri] int key)
        //{
        //    return SingleResult.Create(db.Payments.Where(m => m.id == key).Select(m => m.Modifier));
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PaymentExists(int key)
        {
            return db.Payments.Count(e => e.id == key) > 0;
        }
    }
}
