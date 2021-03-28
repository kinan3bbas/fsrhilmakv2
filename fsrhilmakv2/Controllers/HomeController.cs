using fsrhilmakv2.Extra;
using fsrhilmakv2.Extras;
using fsrhilmakv2.Models;
using fsrhilmakv2.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;


namespace fsrhilmakv2.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private UserHelperLibrary helper = new UserHelperLibrary();
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();
        //public ActionResult Index()
        //{
        //    ViewBag.Title = "Home Page";
        //    StatisticsMainPage result = new StatisticsMainPage();
        //    result.statistics = db.Statistics.OrderByDescending(a => a.CreationDate).FirstOrDefault();

        //    result.services = db.Services.Where(a => a.Status.Equals("Done")
        //    && !a.PrivateService)
        //    .Include(a => a.ServiceProvider)
        //    .Include(a => a.ServicePath)
        //    .Take(5).ToList();
        //    return View(result);
        //}
        public ActionResult Services(int idd, int? page, string status="Done")
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);


            return View(db.Services.Where(a => a.UserWorkId.Equals(idd) && !a.PrivateService&&a.Status.Equals(status))
                .Include(a => a.ServiceProvider)
                .Include(a => a.ServicePath).OrderByDescending(a=>a.CreationDate).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Service(int idd)
        {
            ServiceResult result = new ServiceResult();

            ViewBag.Title = "الخدمة";
            Service service = db.Services.Where(a => a.id.Equals(idd))
                .Include(a => a.ServiceProvider)
                .Include(a => a.ServicePath)
                .FirstOrDefault();
            result.service = service;
            result.services = db.Services.Where(a => a.Status.Equals("Done") && a.ServiceProviderId.Equals(service.ServiceProviderId) 
            && !a.PrivateService)
            .Include(a => a.ServiceProvider)
            .Include(a => a.ServicePath)
            .Take(3).ToList();
            service.numberOfViews++;
            db.Entry(service).State = EntityState.Modified;
            db.SaveChanges();
            return View(result);
        }

        public ActionResult Main()
        {
            Statistics result = db.Statistics.Where(a => a.status.Equals("Active")).FirstOrDefault();

            return View(result);
        }

        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult Index()
        {
            StatisticsMainPage result = new StatisticsMainPage();
            result.statistics = db.Statistics.OrderByDescending(a => a.CreationDate).FirstOrDefault();

            result.services = db.Services.Where(a => a.Status.Equals("Done")
            && !a.PrivateService)
            .Include(a => a.ServiceProvider)
            .Include(a => a.ServicePath)
            .Take(5).ToList();
            return View(result);

        }
        public ActionResult ServiceProviders(int idd)
        {
            int count = db.UserWorkBindings.Where(a => a.UserWorkId.Equals(idd) && a.User.Status.Equals("Active") && a.User.verifiedInterpreter
            ).Count();
            List<String> bindings = db.UserWorkBindings.Where(a => a.UserWorkId.Equals(idd) && a.User.Status.Equals("Active")
            && a.User.Type.Equals(CoreController.UserType.Service_Provider.ToString()) && a.User.verifiedInterpreter
            ).OrderByDescending(a => a.CreationDate).Include(a => a.User).Select(a => a.User.Id).ToList(); ;
            List<UserInfoCash> users = db.UserInfoCashs.ToList();
            return View(users.Where(a => bindings.Contains(a.Id)).OrderByDescending(a => a.Speed));
        }

        [HttpGet]
        public JsonResult generateStatisics()
        {
            List<ApplicationUser> users = db.Users.Where(a => a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
            List<ApplicationUser> Clients = db.Users.Where(a => a.Type.Equals(CoreController.UserType.Client.ToString()) &&
                a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
            List<ApplicationUser> ServiceProviders = db.Users.Where(a => a.Type.Equals(CoreController.UserType.Service_Provider.ToString()) &&
                a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
            List<Service> AllServices = db.Services.ToList();


            Statistics result = new Statistics();
            result.AllClients = Clients.Count();

            result.AllUsers = users.Count();
            result.AllServiceProviders = ServiceProviders.Count();
            result.AllActiveClients = result.AllClients - (int)(result.AllClients * 0.8);
            result.AllActiveServices = AllServices.Where(a => a.Status.Equals(CoreController.ServiceStatus.Active.ToString())).Count();
            result.AllDoneServices = AllServices.Where(a => a.Status.Equals(CoreController.ServiceStatus.Done.ToString())).Count();
            result.AllServices = AllServices.Count();
            Random random = new Random();
            //result.AllActiveClientsInThePastThreeDays = result.AllClients + (int)(result.AllClients * 0.7);

            result.AllDreamUsers = helper.getServiceProviders(CoreController.UserWorkCode.Dream.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllRouqiaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Rouqia.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllIftaaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Iftaa.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllIstasharaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Istishara.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllMedicalUsers = helper.getServiceProviders(CoreController.UserWorkCode.Medical.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllLawUsers = helper.getServiceProviders(CoreController.UserWorkCode.Law.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.CreationDate = DateTime.Now;
            result.LastModificationDate = DateTime.Now;
            result.status = "Active";
            Statistics current = db.Statistics.Where(a => a.status.Equals("Active")).FirstOrDefault();
            if (current != null)
            {
                current.status = "Not_Active";
                db.Entry(current).State = System.Data.Entity.EntityState.Modified;
            }

            db.Statistics.Add(result);

            db.SaveChanges();
            return Json("200", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ServiceProvider(String type)
        {
            return View();
        }

        public IReadOnlyCollection<SitemapNode> GetSitemapNodes(UrlHelper urlHelper,bool secondFile)
        {
            List<SitemapNode> nodes = new List<SitemapNode>();

            if (!secondFile)
            {
                nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.AbsoluteRouteUrl("AboutUs", "Home", new { controller = "Home" }),
                    Priority = 1
                });
                nodes.Add(
                   new SitemapNode()
                   {
                       Url = urlHelper.AbsoluteRouteUrl("index", "Home", new { controller = "Home", action = "index" }),
                       Priority = 0.9
                   });


                foreach (int productId in db.Services.OrderBy(a => a.CreationDate).Take(45000).Select(a => a.id))
                {
                    nodes.Add(
                       new SitemapNode()
                       {
                           Url = urlHelper.AbsoluteRouteUrl("Service", "Home", new { idd = productId }),
                           Frequency = SitemapFrequency.Weekly,
                           Priority = 0.8
                       });
                }

            }
            else
            {
                foreach (int productId in db.Services.OrderBy(a => a.CreationDate).Skip(49500).Select(a => a.id))
                {
                    nodes.Add(
                new SitemapNode()
                {
                    Url = urlHelper.AbsoluteRouteUrl("AboutUs", "Home", new { controller = "Home" }),
                    Priority = 1
                });
                    nodes.Add(
                       new SitemapNode()
                       {
                           Url = urlHelper.AbsoluteRouteUrl("index", "Home", new { controller = "Home", action = "index" }),
                           Priority = 0.9
                       });
                    nodes.Add(
                       new SitemapNode()
                       {
                           Url = urlHelper.AbsoluteRouteUrl("Service", "Home", new { idd = productId }),
                           Frequency = SitemapFrequency.Weekly,
                           Priority = 0.8
                       });
                }
            }
            return nodes;
        }

        public string GetSitemapDocument(IEnumerable<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                    sitemapNode.LastModified == null ? null : new XElement(
                        xmlns + "lastmod",
                        sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    sitemapNode.Frequency == null ? null : new XElement(
                        xmlns + "changefreq",
                        sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null ? null : new XElement(
                        xmlns + "priority",
                        sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);
            return document.ToString();
        }

        [Route("sitemap.xml")]
        public ActionResult SitemapXml()
        {
            var sitemapNodes = GetSitemapNodes(this.Url,false);
            string xml = GetSitemapDocument(sitemapNodes);
            return this.Content(xml, MediaTypeNames.Text.Xml, Encoding.UTF8);
        }

        [Route("sitemap2.xml")]
        public ActionResult SitemapXml2()
        {
            var sitemapNodes = GetSitemapNodes(this.Url,true);
            string xml = GetSitemapDocument(sitemapNodes);
            return this.Content(xml, MediaTypeNames.Text.Xml, Encoding.UTF8);
        }


    }
}
