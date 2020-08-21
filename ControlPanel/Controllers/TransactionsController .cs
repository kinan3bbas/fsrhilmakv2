
using ControlPanel.Extra;
using ControlPanel.Extras;
using ControlPanel.Models;
using ControlPanel.ViewModels;
using fsrhilmakv2.Extra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;



namespace ControlPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TransactionsController : Controller
    {
        private UserHelperLibrary helper = new UserHelperLibrary();




        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index( string status, String fromDate = "", String toDate = "")

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
            List<Transaction> transactions = db.Transactions
                .Include("User")
                

                .ToList();
            transactions = transactions.Where(a => a.CreationDate.CompareTo(from) >= 0 && a.CreationDate.CompareTo(to) <= 0).ToList();
            return View(transactions);
        }


        public ActionResult Create(String userId)
        {
            Transaction trans = new Transaction();
            trans.UserId = userId;
            return View(trans);
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Transaction Transaction)
        {
            if (Transaction.UserId == null)
            {
                return View(Transaction);
            }
            if (ModelState.IsValid)
            {
                Transaction.CreationDate = DateTime.Now;
                Transaction.LastModificationDate = DateTime.Now;
                Transaction.Status = "Done";
                db.Transactions.Add(Transaction);
                db.SaveChanges();
                return RedirectToAction("PersonalPage", "Account", new { userId = Transaction.UserId });
            }
            return View(Transaction);
        }




    }
}
