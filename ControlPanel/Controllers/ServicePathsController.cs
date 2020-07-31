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
    [Authorize]
    public class ServicePathsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ServicePaths
        public ActionResult Index()
        {
            var servicePaths = db.ServicePaths.Include(s => s.Creator).Include(s => s.Modifier).Include(s => s.ServiceProvider);
            return View(servicePaths.ToList());
        }

        // GET: ServicePaths/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServicePath servicePath = db.ServicePaths.Find(id);
            if (servicePath == null)
            {
                return HttpNotFound();
            }
            return View(servicePath);
        }

        // GET: ServicePaths/Create
        public ActionResult Create()
        {
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "Sex");
            ViewBag.ModifierId = new SelectList(db.Users, "Id", "Sex");
            ViewBag.ServiceProviderId = new SelectList(db.Users, "Id", "Sex");
            return View();
        }

        // POST: ServicePaths/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServicePath servicePath)
        {
            if (ModelState.IsValid)
            {
                servicePath.CreationDate = DateTime.Now;
                servicePath.LastModificationDate = DateTime.Now;
                db.ServicePaths.Add(servicePath);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatorId = new SelectList(db.Users, "Id", "Sex", servicePath.CreatorId);
            ViewBag.ModifierId = new SelectList(db.Users, "Id", "Sex", servicePath.ModifierId);
            ViewBag.ServiceProviderId = new SelectList(db.Users, "Id", "Sex", servicePath.ServiceProviderId);
            return View(servicePath);
        }

        // GET: ServicePaths/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServicePath servicePath = db.ServicePaths.Find(id);
            if (servicePath == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "Sex", servicePath.CreatorId);
            ViewBag.ModifierId = new SelectList(db.Users, "Id", "Sex", servicePath.ModifierId);
            ViewBag.ServiceProviderId = new SelectList(db.Users, "Id", "Sex", servicePath.ServiceProviderId);
            return View(servicePath);
        }

        // POST: ServicePaths/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServicePath servicePath)
        {
            if (ModelState.IsValid)
            {
                ServicePath origin = db.ServicePaths.Where(y => y.id == servicePath.id).First();
                origin.Name = servicePath.Name;
                origin.Cost = servicePath.Cost;
                origin.Enabled = servicePath.Enabled;
                origin.LastModificationDate = DateTime.Now;
                origin.Message = servicePath.Message;
                origin.Ratio = servicePath.Ratio;
                db.Entry(origin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "Sex", servicePath.CreatorId);
            ViewBag.ModifierId = new SelectList(db.Users, "Id", "Sex", servicePath.ModifierId);
            ViewBag.ServiceProviderId = new SelectList(db.Users, "Id", "Sex", servicePath.ServiceProviderId);
            return View(servicePath);
        }

        // GET: ServicePaths/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServicePath servicePath = db.ServicePaths.Find(id);
            if (servicePath == null)
            {
                return HttpNotFound();
            }
            return View(servicePath);
        }

        // POST: ServicePaths/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServicePath servicePath = db.ServicePaths.Find(id);
            db.ServicePaths.Remove(servicePath);
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
