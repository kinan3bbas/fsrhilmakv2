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
        public ActionResult Index()
        {


            var competitions = db.Competitions.Include(s => s.UserWork);
            return View(competitions.ToList());
        }

        // GET: Competitions/Details/5
        public ActionResult Details(int? id)
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
        public ActionResult Edit(int? id)
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

        // POST: Competitions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Competition Competition)
        {
            if (ModelState.IsValid)
            {
                Competition temp = db.Competitions.Find(Competition.id);
                temp.Name = Competition.Name;
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
