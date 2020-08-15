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
    public class SystemParametersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SystemParameters
        public async Task<ActionResult> Index()
        {
            return View(await db.SystemParameters.ToListAsync());
        }

        // GET: SystemParameters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemParameter systemParameter = db.SystemParameters.Find(id);
            if (systemParameter == null)
            {
                return HttpNotFound();
            }
            return View(systemParameter);
        }

        // GET: SystemParameters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemParameters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Name,Code,Value")] SystemParameter systemParameter)
        {
            if (ModelState.IsValid)
            {
                systemParameter.CreationDate = DateTime.Now;
                systemParameter.LastModificationDate = DateTime.Now;
                db.SystemParameters.Add(systemParameter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(systemParameter);
        }

        // GET: SystemParameters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemParameter systemParameter = db.SystemParameters.Find(id);
            if (systemParameter == null)
            {
                return HttpNotFound();
            }
            return View(systemParameter);
        }

        // POST: SystemParameters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SystemParameter systemParameter)
        {
            if (ModelState.IsValid)
            {
                SystemParameter temp = db.SystemParameters.Find(systemParameter.id);
                temp.Name = systemParameter.Name;
                temp.Code = systemParameter.Code;
                temp.Value = systemParameter.Value;
                temp.LastModificationDate = DateTime.Now;
                
                db.Entry(temp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(systemParameter);
        }

        // GET: SystemParameters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemParameter systemParameter = db.SystemParameters.Find(id);
            if (systemParameter == null)
            {
                return HttpNotFound();
            }
            return View(systemParameter);
        }

        // POST: SystemParameters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SystemParameter systemParameter = db.SystemParameters.Find(id);
            db.SystemParameters.Remove(systemParameter);
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
