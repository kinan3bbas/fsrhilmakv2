using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fsrhilmakv2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Main()
        {
            //ViewBag.numberOfClients = db.Users.Where(a => a.Type.Equals("Client")).Count();
            //ViewBag.numberOfInterpreters = db.Users.Where(a => a.Type.Equals("Interpreter") && a.verifiedInterpreter && !a.Status.Equals("Deleted")).Count();
            //ViewBag.numberOfActiveUsers = db.Users.Where(a => a.Status.Equals("Active")).Count();
            //ViewBag.numberOfDoneDreams = db.Dreams.Where(a => a.Status.Equals("Done")).Count();
            //ViewBag.numberOfActiveDreams = db.Dreams.Where(a => a.Status.Equals("Active")).Count();
            //ViewBag.numberOfVisits = db.WebSiteStatistics.Sum(a => a.numberOfVisits);
            //ViewBag.numberOfViews = db.Dreams.Sum(a => a.numberOfViews);
            //ViewBag.numberOfLikes = db.Dreams.Sum(a => a.numberOfLikes);
            //WebSiteStatistics temp = new WebSiteStatistics();
            //temp.numberOfVisits = 1;
            //webSiteStatisticsRepository.Create(temp);
            return View();
        }
    }
}
