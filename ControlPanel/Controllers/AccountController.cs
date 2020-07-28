using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ControlPanel.Models;
using ControlPanel.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;

namespace ControlPanel.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                user.CreationDate = DateTime.Now;
                user.LastModificationDate = DateTime.Now;
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }


        // GET: /Account/ServiceProvider
        public ActionResult ServiceProvider(int? UserWorkId, String fromDate = "", String toDate = "")
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
            List<ApplicationUser> users = db.Users.Where(a => a.Type== "Service_Provider"
                    && a.Status!="Deleted").ToList();
            if (UserWorkId != null)
            {
                List<UserWorkBinding> bindings = db.UserWorkBindings.Where(a => a.UserWorkId == UserWorkId
                        ).Include("User").ToList();
                users = bindings.Select(a => a.User).ToList();
                users = users.Where(a => a.CreationDate.CompareTo(from) >= 0 && a.CreationDate.CompareTo(to) <= 0).ToList();

            }
            List<UserInfoViewModel> result = new List<UserInfoViewModel>();
            foreach (var item in users)
            {
                result.Add(getInfoMapping(item));
            }
            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a=>a.Enabled), "id", "Name");
            return View(result);
        }

        //// GET: /Account/Clients
        [HttpGet]
        public ActionResult Clients(int? UserWorkId, String fromDate = "", String toDate = "")
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

            List<ApplicationUser> users = db.Users.Where(a => a.Type.Equals(CoreController.UserType.Client.ToString())
            && !a.Status.Equals(CoreController.UserStatus.Deleted.ToString())).ToList();
            users = users.Where(a => a.CreationDate.CompareTo(from) >= 0&& a.CreationDate.CompareTo(to) <= 0).ToList();
            List<UserInfoViewModel> result = new List<UserInfoViewModel>();
            foreach (var item in users)
            {
                result.Add(getInfoMapping(item));
            }
            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled), "id", "Name");

            return View(result);
        }

        // GET: /Account/DeletdUsers
        [HttpGet]

        public ActionResult DeletedUsers()
        {
            var users = db.Users.Where(a => a.Status.Equals(CoreController.UserStatus.Deleted.ToString())).ToList();
            List<UserInfoViewModel> result = new List<UserInfoViewModel>();
            foreach (var item in users)
            {
                result.Add(getInfoMapping(item));
            }
            return View(result);
        }

        public UserInfoViewModel getInfoMapping(ApplicationUser user)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            List<UserWorkBinding> userWork = db.UserWorkBindings.Where(a => a.UserId.Equals(user.Id)).Include("UserWork").ToList();
            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                Age = user.Age,
                Country = user.Country,
                JobDescription = user.JobDescription,
                JoiningDate = user.JoiningDate,
                Name = user.Name,
                MartialStatus = user.MartialStatus,
                PictureId = user.PictureId,
                Sex = user.Sex,
                Status = user.Status,
                Type = user.Type,
                phoneNumber = user.PhoneNumber,
                PersonalDescription = user.PersonalDescription,
                FireBaseId = user.FireBaseId,
                Id = user.Id,
                HasRegistered = user.verifiedInterpreter,
                UserWorks = userWork.Select(a => a.UserWork).ToList(),
                NumberOfActiveServices = 10,
                NumberOfDoneServices = 10,
                Speed = 10,
                AvgServicesInOneDay = 10,
                UserRoles = userManager.GetRoles(user.Id).ToList(),


            };
        }


        public ActionResult PersonalPage(String userId)
        {
            ApplicationUser temp = db
                .Users
                .Where(e => e.Id.Equals(userId))
                .Include("userWorkBinding")
                .FirstOrDefault();
            if (temp == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = userId;
            return View(getInfoMapping(temp));
        }






        public ActionResult EditPersonalPage(String id)
        {
            ApplicationUser temp = db
                .Users
                .Where(e => e.Id.Equals(id))
                .FirstOrDefault();
            if (temp == null)
            {
                return HttpNotFound();
            }
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //var user = userManager.FindById(User.Identity.GetUserId());
            //ViewBag.CurrentUser = user;

            ViewBag.userId = id;
            return View(temp);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPersonalPage(ApplicationUser user)
        {
            if (user == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                ApplicationUser temp = db.Users.Where(a => a.Id.Equals(user.Id)).FirstOrDefault();
                temp.Id = user.Id;
                temp.Name = user.Name;
                temp.UserName = user.UserName;
                temp.Age = user.Age;
                temp.PhoneNumber = user.PhoneNumber;
                temp.Status = user.Status;
                temp.Sex = user.Sex;
                temp.Country = user.Country;
                temp.MartialStatus = user.MartialStatus;
                temp.Type = user.Type;
                db.Entry(temp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PersonalPage", new { id = user.Id });

            }
            return View(user)
;
        }

        public ActionResult Ratings(String userId)
        {
            List<Service> ratings = db.Services.Where(a => a.ServiceProviderId.Equals(userId)).
                Include(s => s.Creator).Include(s => s.Modifier).Include(s => s.ServiceProvider).ToList();
            if (ratings == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = userId;
            return View(ratings);
        }



        public ActionResult ServiceHistory(String userId)
        {
            List<DreamHistory> dreamhistory = db.DreamHistorys.Where(a => a.Service.ServiceProviderId.Equals(userId)).
                Include(s => s.Creator).Include(s => s.Modifier).Include(s => s.Service.ServiceProvider).ToList();
            if (dreamhistory == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = userId;
            return View(dreamhistory);
        }



        // GET:         Transactions

        public ActionResult Transactions(String userId)
        {
            List<Transaction> transaction = db.Transactions.Where(a => a.UserId.Equals(userId)).
                Include(s => s.Creator).Include(s => s.Modifier).Include(s => s.UserId).ToList();

            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = userId;
            return View(transaction);
        }




        // GET: Payments
        public ActionResult Payments(String userId)
        {
            List<Payment> payments = db.Payments.Where(a => a.Service.ServiceProviderId.Equals(userId)).Include(s => s.Creator).Include(s => s.Modifier).Include(s => s.Service.ServiceProvider).ToList();

            if (payments == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = userId;
            return View(payments);
        }

        public ActionResult ServicePath(String userId)
        {

            var servicePaths = db.ServicePaths.Where(a => a.ServiceProviderId.Equals(userId)).Include(s => s.Creator).Include(s => s.Modifier).Include(s => s.ServiceProvider);

            if (servicePaths == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = userId;
            return View(servicePaths.ToList());

        }

        // GET: /CreateServicePath
        public ActionResult CreateServicePath(String ServiceProviderId)
        {
            //ViewBag.CreatorId = new SelectList(db.Users, "Id", "Sex");
            //ViewBag.ModifierId = new SelectList(db.Users, "Id", "Sex");
            //ViewBag.ServiceProviderId = new SelectList(db.Users, "Id", "Sex");
            return View();
        }

        // POST: CreateServicePath
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateServicePath(ServicePath servicePath)
        {
            if (ModelState.IsValid)
            {
                servicePath.CreationDate = DateTime.Now;
                servicePath.LastModificationDate = DateTime.Now;
                db.ServicePaths.Add(servicePath);
                db.SaveChanges();
                return RedirectToAction("ServicePath",new {userId=servicePath.ServiceProviderId });
            }

            
            return View(servicePath);
        }




        // GET: /EditServicePath
        public ActionResult EditServicePath(int? userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServicePath servicePath = db.ServicePaths.Find(userId);
            if (servicePath == null)
            {
                return HttpNotFound();
            }
            return View(servicePath);
        }



        // POST: EditServicePath
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditServicePath(ServicePath servicePath)
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
                return RedirectToAction("ServicePath", new { userId = servicePath.ServiceProviderId });
            }
           
            return View(servicePath);
        }





        // GET: ServicePaths/Delete/5
        public ActionResult Delete(int? userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServicePath servicePath = db.ServicePaths.Find(userId);
            if (servicePath == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("ServicePath", new { userId = servicePath.ServiceProviderId });
        }

        // POST: ServicePaths/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int userId)
        {
            ServicePath servicePath = db.ServicePaths.Find(userId);
            db.ServicePaths.Remove(servicePath);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}