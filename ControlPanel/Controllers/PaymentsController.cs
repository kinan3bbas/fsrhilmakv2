using ControlPanel.Extra;
using ControlPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlPanel.Controllers
{
    public class PaymentsController : Controller
    {
        private UserHelperLibrary helper = new UserHelperLibrary();

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Payments
        public ActionResult Index(int? UserWorkId,  String fromDate = "", String toDate = "")
        {
            DateTime from = new DateTime(2000, 1, 1);
            DateTime to = new DateTime(3000, 1, 1);
            if (!fromDate.Equals("") && fromDate != null)
            {
                DateTime.TryParse(fromDate, out from);
            }
            if (!toDate.Equals("") && toDate != null)
            {
                DateTime.TryParse(toDate, out to);
            }
            List<Payment> payments = db.Payments
                .Include("Service")
                .Include("Service.ServiceProvider")
                .Include("Service.ServicePath")
                .Include("Creator")
                .ToList();

            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled), "id", "AdjectiveName");
            return View(payments);
        }

        // GET: Payments/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Payments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Payments/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Payments/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Payments/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Payments/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Payments/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
