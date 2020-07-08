using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ControlPanel.Models;

namespace ControlPanel.Controllers
{
    public class UserWorksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserWorks
        public ActionResult Index()
        {
            return View(db.UserWorks.ToList());
        }

        // GET: UserWorks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserWork userWork = db.UserWorks.Find(id);
            if (userWork == null)
            {
                return HttpNotFound();
            }
            return View(userWork);
        }

        // GET: UserWorks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserWorks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Name,AdjectiveName,Enabled")] UserWork userWork)
        {
            if (ModelState.IsValid)
            {
                userWork.CreationDate = DateTime.Now;
                userWork.LastModificationDate = DateTime.Now;
                db.UserWorks.Add(userWork);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userWork);
        }

        // GET: UserWorks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserWork userWork = db.UserWorks.Find(id);
            if (userWork == null)
            {
                return HttpNotFound();
            }
            return View(userWork);
        }

        // POST: UserWorks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Name,AdjectiveName,Enabled")] UserWork userWork)
        {
            if (ModelState.IsValid)
            {
                UserWork origin = db.UserWorks.Where(y => y.id == userWork.id).First();
                origin.Name = userWork.Name;
                origin.AdjectiveName = userWork.AdjectiveName;
                origin.Enabled = userWork.Enabled;
                origin.LastModificationDate = DateTime.Now;
                db.Entry(origin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userWork);
        }

        // GET: UserWorks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserWork userWork = db.UserWorks.Find(id);
            if (userWork == null)
            {
                return HttpNotFound();
            }
            return View(userWork);
        }

        // POST: UserWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserWork userWork = db.UserWorks.Find(id);
            db.UserWorks.Remove(userWork);
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
