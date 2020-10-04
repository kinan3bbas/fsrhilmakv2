using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ControlPanel.Models;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
 [Authorize(Roles = "Admin")]

    public class CompetitionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Competitions
        public ActionResult Index(int? UserWorkId, string status, string goal, String fromDate = "", String toDate = "")
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
            List<Competition> competitions = db.Competitions.Include("UserWork").ToList();
            

            if (status != null && !status.Equals(""))
                competitions = competitions.Where(a => a.Status.Equals(status)).OrderByDescending(r => r.CreationDate).ToList();
            //else
            //    competitions = competitions.Where(a => a.Status.Equals("Active")).OrderByDescending(r => r.CreationDate).ToList();

            if (goal != null && !goal.Equals(""))
                competitions = competitions.Where(a => a.Goal.Equals(goal)).OrderByDescending(r => r.CreationDate).ToList();
            

           
            if (UserWorkId != null)
            {

                competitions = competitions.Where(a => a.UserWorkId.Equals(UserWorkId)).OrderByDescending(r => r.CreationDate).ToList();
            }
            competitions = competitions.Where(a => a.CreationDate.CompareTo(from) >= 0 && a.CreationDate.CompareTo(to) <= 0).ToList();
            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled), "id", "AdjectiveName");

            return View(competitions.ToList());
        }

        // GET: Competitions/Details/5
        public ActionResult Details(int id)
        {

          
            var Competition = db.Competitions.Where(e => e.id.Equals(id))
               . Include("UserWork")
                .FirstOrDefault();
            if (Competition == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserWorkId = id;
            return View(Competition);
        }


        // GET: Competitions/Create
        public ActionResult Create()
        {
            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled), "id", "AdjectiveName");
            return View();
        }

        // POST: Competitions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Competition Competition)
        {
            if (ModelState.IsValid)
            {
                Competition.Status = "active";
                Competition.CreationDate = DateTime.Now;
                Competition.LastModificationDate = DateTime.Now;
                db.Competitions.Add(Competition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Competition);
        }

        // GET: Competitions/Edit/5
        public ActionResult Edit(int id)
        {
           
            var competition = db.Competitions.Where(a => a.id.Equals(id))
               .Include("UserWork")
                .FirstOrDefault();
            if (competition == null)
            {
                return HttpNotFound();
            }
            return View(competition);
        }

        // POST: Competitions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Competition Competition)
        {
            if (ModelState.IsValid)
            {

                Competition temp = db.Competitions.Where(a => a.id.Equals(Competition.id)). Include("UserWork").FirstOrDefault();
                temp.Name = Competition.Name;
                temp.Status = Competition.Status;
                temp.UserWork.Name = Competition.UserWork.Name;
                temp.Goal = Competition.Goal;
                temp.StartDate = Competition.StartDate;
                temp.EndDate = Competition.EndDate;
                temp.LastModificationDate = DateTime.Now;                
                db.Entry(temp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Competition);
        }

        // GET: Competitions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competition Competition = db.Competitions.Find(id);
            if (Competition == null)
            {
                return HttpNotFound();
            }
            return View(Competition);
        }

        // POST: Competitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Competition Competition = db.Competitions.Find(id);
            db.Competitions.Remove(Competition);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
