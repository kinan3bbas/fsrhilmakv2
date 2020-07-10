using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ControlPanel.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ControlPanel.Controllers
{
    [Authorize]
    [RoutePrefix("api/Core")]
    public class CoreController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public  enum UserStatus { Active, Not_Active, Deleted };

        public  enum UserType { Service_Provider, Client, Admin };

        public enum ServiceStatus { Active,Done, Deleted };

        public CoreController() { }

        




        public ApplicationUser getCurrentUser ()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            return  userManager.FindById(User.Identity.GetUserId());
            
        }

    }
}
