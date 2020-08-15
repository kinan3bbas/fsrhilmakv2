using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using fsrhilmakv2.Models;
using fsrhilmakv2.Providers;
using fsrhilmakv2.Results;
using System.Linq;
using System.Data.Entity;
using fsrhilmakv2.ViewModels;
using fsrhilmakv2.Extra;

namespace fsrhilmakv2.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();
        private UserHelperLibrary helper = new UserHelperLibrary();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            ApplicationUser user = core.getCurrentUser();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //UsersDeviceTokens token = db.UsersDeviceTokens.Where(a => a.UserId.Equals(user.Id)).FirstOrDefault();
            if (user.Type.Equals(CoreController.UserType.Client.ToString()))
            {
                return new UserInfoViewModel
                {
                    Email = User.Identity.GetUserName(),
                    Name = user.Name,
                    Status = user.Status,
                    Type = user.Type,
                    phoneNumber = user.PhoneNumber,
                    Id = user.Id,
                    UserName = user.UserName,
                    SocialStatus = user.SocialState,
                    UserRoles = userManager.GetRoles(user.Id).ToList(),
                    UserSpecialCode = user.UserSpecialCode,
                    PointsBalance = user.PointsBalance,
                    UserRegistrationCode = user.UserRegistrationCode,
                    PictureId=user.PictureId
                };
            }

            List<UserWorkBinding> userWork = db.UserWorkBindings.Where(a => a.UserId.Equals(user.Id)).Include("UserWork").ToList();
            return getInfoMapping(user);
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            core.ChangeOnlineStatus(core.getCurrentUser(), false);
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            

            ApplicationUser tempUserUserName = db.Users.Where(a => a.UserName.Equals(model.Username)).FirstOrDefault();
            if (tempUserUserName != null)
                await core.throwExcetpion("Username is already taken!");

            if (model.PhoneNumber != null) {
                ApplicationUser tempUserPhoneNUmber = db.Users.Where(a => a.PhoneNumber.Equals(model.PhoneNumber)).FirstOrDefault();
                if (tempUserPhoneNUmber != null)
                    await core.throwExcetpion("phone number is already taken!");
            }
            

            ApplicationUser tempUserEmail = db.Users.Where(a => a.Email.Equals(model.Email)).FirstOrDefault();
            if (tempUserEmail != null)
                await core.throwExcetpion("Email is already taken!");
            IdentityResult result = null;
            if (model.SocialState == "Normal")
            {
                //Admin User
                if (model.Type.Equals(CoreController.UserType.Admin.ToString()))
                {
                    var user = new ApplicationUser()
                    {
                        UserName = model.Username,
                        Email = model.Email,
                        Name = model.Name,
                        Status = CoreController.UserStatus.Active.ToString(),
                        Type = model.Type,
                        CreationDate = DateTime.Now,
                        FireBaseId = model.FireBaseId,
                        LastModificationDate = DateTime.Now

                    };
                    result = await UserManager.CreateAsync(user, model.Password);
                    //await UserManager.AddToRoleAsync(user.Id, "Admin");
                }


                //Cliet User
                if (model.Type.Equals(CoreController.UserType.Client.ToString()))
                {
                    var user = new ApplicationUser()
                    {
                        UserName = model.Username,
                        Email = model.Email,
                        Name = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        Status = CoreController.UserStatus.Active.ToString(),
                        Type = model.Type,
                        CreationDate = DateTime.Now,
                        LastModificationDate = DateTime.Now,
                        SocialState = model.SocialState,
                        PictureId = model.PictureId
                    };

                    if (model.UserRegistrationCode != null && !model.UserRegistrationCode.Equals(""))
                    {
                        user.UserRegistrationCode = model.UserRegistrationCode;
                        addPoints(model);
                    }
                    GenerateUserSpecialCode(user);
                    result = await UserManager.CreateAsync(user, model.Password);
                   // await UserManager.AddToRoleAsync(user.Id, "Client");
                }


                // Interpreter
                if (model.Type.Equals(CoreController.UserType.Service_Provider.ToString()))
                {
                    var user = new ApplicationUser()
                    {
                        UserName = model.Username,
                        Name = model.Name,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        MartialStatus = model.MartialStatus,
                        Sex = model.Sex,
                        Age = model.Age,
                        JobDescription = model.JobDescription,
                        PersonalDescription = model.PersonalDescription,
                        Country = model.Country,
                        JoiningDate = DateTime.Now,
                        PictureId = model.PictureId,
                        Status = CoreController.UserStatus.Active.ToString(),
                        Type = model.Type,
                        CreationDate = DateTime.Now,
                        LastModificationDate = DateTime.Now,
                        verifiedInterpreter = false
                    };



                    //List<int> userworksIds = new List<int>();
                    //List<UserWork> userworks = db.UserWorks.ToList();
                    //ICollection<UserWorkBinding> userWorksToBind = new List<UserWorkBinding>();
                    //if (model.UserWork != null && model.UserWork.Count > 0)
                    //{

                    //    foreach (var item in model.UserWork)
                    //    {
                    //        //db.Entry(item).State = EntityState.Deleted;
                    //        UserWorkBinding temp = new UserWorkBinding
                    //        {
                    //            CreationDate = DateTime.Now,
                    //            LastModificationDate = DateTime.Now,
                    //            UserId = user.Id,
                    //            // User = user,
                    //            //UserWork = userworks.Where(a => a.id.Equals(item.id)).ToList()[0],
                    //            UserWorkId = item.id

                    //        };
                    //        userworksIds.Add(item.id);
                    //        db.UserWorkBindings.Add(temp);

                    //        userWorksToBind.Add(temp);
                    //    }
                    //    user.userWorkBinding = userWorksToBind;
                    //    //db.Entry(user).State = EntityState.Modified;
                    //    //db.SaveChanges();
                    //}

                    result = await UserManager.CreateAsync(user, model.Password);
                    //int j = 0;
                    //List<UserWorkBinding> bindings = user.userWorkBinding.ToList(); ;
                    //foreach (var item in bindings)
                    //{
                    //    db.UserWorks.Remove(item.UserWork);
                    //    //item.UserWorkId = userworksIds[j];
                    //    j++;
                    //    db.Entry(item).State = EntityState.Modified;
                    //}
                    db.SaveChanges();
                   // await UserManager.AddToRoleAsync(user.Id, "Service_Provider");
                }
                
            }
            else
            {
                //Facebook User
                if (model.Type.Equals(CoreController.UserType.Client.ToString()))
                {
                    var user = new ApplicationUser()
                    {
                        UserName = model.Username,
                        Email = model.Email,
                        Name = model.Name,
                        Status = CoreController.UserStatus.Active.ToString(),
                        Type = model.Type,
                        CreationDate = DateTime.Now,
                        LastModificationDate = DateTime.Now,
                        SocialState = model.SocialState,
                        SocialToken = model.SocialToken,
                        imageUrl = model.imageUrl
                    };

                    if (model.UserRegistrationCode != null && !model.UserRegistrationCode.Equals(""))
                    {
                        user.UserRegistrationCode = model.UserRegistrationCode;
                        addPoints(model);
                    }
                    GenerateUserSpecialCode(user);
                    result = await UserManager.CreateAsync(user, model.Password);
                    // await UserManager.AddToRoleAsync(user.Id, "Client");
                }
            }
            //IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok(result);
        }


        public void addPoints(RegisterBindingModel model)
        {
            ApplicationUser winner = db.Users.Where(a => a.UserSpecialCode.Equals(model.UserRegistrationCode)).FirstOrDefault();
            if (winner != null)
            {
                SystemParameter parm = db.SystemParameters.Where(a => a.Code.Equals("PointPrice")).FirstOrDefault();
                
                winner.PointsBalance = winner.PointsBalance + (parm!=null?long.Parse(parm.Value.ToString()):5);
                winner.LastModificationDate = DateTime.Now;
                db.Entry(winner).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void GenerateUserSpecialCode(ApplicationUser user)
        {
            user.UserSpecialCode = UserVerificationHelper.GenerateCode();
        }
        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        [AllowAnonymous]
        [Route("GetSingleUserInfo")]
        public UserInfoViewModel GetSingleUserInfo([FromUri]string id)
        {

            ApplicationUser user = db.Users.Find(id);
            return getInfoMapping(user);
        }

        public UserInfoViewModel getInfoMapping(ApplicationUser user)
        {
            List<Service> services = helper.getUserServices(user.Id);
            List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
            List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            List<UserWorkBinding> userWork = db.UserWorkBindings.Where(a => a.UserId.Equals(user.Id)).Include("UserWork").ToList();
            double speed = UserHelperLibrary.ServiceProviderSpeed(helper.findUser(user.Id), doneServices.Count);
            double avg= UserHelperLibrary.ServiceProviderAvgServices(helper.findUser(user.Id), services.Count);
            UserBalance balance = helper.getUserBalance(user);
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
                UserWorks = userWork,
                NumberOfActiveServices = activeSerives.Count(),
                NumberOfDoneServices = doneServices.Count(),
                Speed = speed<1?1:speed,
                AvgServicesInOneDay = avg == 0 ? 1 : avg,
                UserRoles = userManager.GetRoles(user.Id).ToList(),
                SocialStatus = user.SocialState,
                ImageUrl = user.imageUrl,
                SocialToken = user.SocialToken,
                TotalBalance = balance.TransferedBalance,
                AvailableBalance = balance.DoneBalance,
                SuspendedBalance = balance.SuspendedBalance
                
                

            };
        }

        public UserInfoViewModel getInfoMapping2(ApplicationUser user)
        {
            List<Service> services = helper.getUserServices(user.Id);
            List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
            List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //List<UserWorkBinding> userWork = db.UserWorkBindings.Where(a => a.UserId.Equals(user.Id)).Include("UserWork").ToList();
            //double speed = UserHelperLibrary.ServiceProviderSpeed(helper.findUser(user.Id), doneServices.Count);
            //double avg = UserHelperLibrary.ServiceProviderAvgServices(helper.findUser(user.Id), services.Count);
            //UserBalance balance = helper.getUserBalance(user);
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
                //UserWorks = userWork,
                NumberOfActiveServices = activeSerives.Count(),
                NumberOfDoneServices = doneServices.Count(),
              //  Speed = speed < 1 ? 1 : speed,
                //AvgServicesInOneDay = avg == 0 ? 1 : avg,
                //UserRoles = userManager.GetRoles(user.Id).ToList(),
                SocialStatus = user.SocialState,
                ImageUrl = user.imageUrl,
                SocialToken = user.SocialToken,
                //TotalBalance = balance.TransferedBalance,
                //AvailableBalance = balance.DoneBalance,
                //SuspendedBalance = balance.SuspendedBalance



            };
        }

        [AllowAnonymous]
        [Route("GetServiceProviders")]
        public IHttpActionResult GetServiceProviders([FromUri]int id,int skip=0,int top=10)
        {
            skip = skip == null ? 0 : skip;
            top = top == null ? 5 : top;
            int count = db.UserWorkBindings.Where(a => a.UserWorkId.Equals(id)
            ).Count();
            List <UserWorkBinding> bindings = db.UserWorkBindings.Where(a => a.UserWorkId.Equals(id)
            ).OrderByDescending(a=>a.CreationDate).Skip(skip).Take(top).Include("User").ToList();
            List<UserInfoViewModel> users = new List<UserInfoViewModel>();
            foreach (var item in bindings)
            {
                users.Add(getInfoMapping2(item.User));
            }
            var genericResutl = new {Users=users,Count=count };
            return Ok(genericResutl);
        }

        [AllowAnonymous]
        [Route("GetServiceProvidersWithoutFilter")]
        public List<UserInfoViewModel> GetServiceProvidersWithoutFilter()
        {
            List<ApplicationUser> result = db.Users.Where(a => a.Type.Equals(CoreController.UserType.Service_Provider.ToString())
              && !a.Status.Equals(CoreController.UserStatus.Deleted.ToString())).ToList();
            List<UserInfoViewModel> users = new List<UserInfoViewModel>();
            foreach (var item in result)
            {
                users.Add(getInfoMapping(item));
            }
            return users;
        }

        [Route("GetServicePathsForProvider")]
        public List<ServicePathViewModel> GetServicePathsForProvider([FromUri]String id)
        {
            List<Service> services = helper.getUserServices(id);
            List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
            List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
            double speed = UserHelperLibrary.ServiceProviderSpeed(helper.findUser(id),doneServices.Count);
            List<ServicePath> paths = db.ServicePaths.Where(a => a.ServiceProviderId.Equals(id)).ToList();
            if (paths.Count == 0)
            {
                paths= db.ServicePaths.Where(a => a.ServiceProviderId.Equals(null) && a.Enabled).ToList();
            }

            List<ServicePathViewModel> result = new List<ServicePathViewModel>();
            foreach (var item in paths)
            {
                int numOfOpenDreams= activeSerives.Where(a => a.ServicePathId.Equals(item.id)).Count();
                ServicePathViewModel temp = new ServicePathViewModel();
                temp.id = item.id;
                temp.Cost = item.Cost;
                temp.Name = item.Name;
                temp.NumberOfPeopleWaiting = numOfOpenDreams;
                temp.AvgWaitingTime= UserHelperLibrary.getWaitingTimeMessage(Double.Parse(speed.ToString()),
                Double.Parse(numOfOpenDreams.ToString())).Replace("Your average waiting time is ", "");
                result.Add(temp);
            }
            return result;
        }

        public ServicePathViewModel GetServicePathForProvider(String id,int pathId)
        {
            List<Service> services = helper.getUserServices(id);
            List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
            List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
            double speed = UserHelperLibrary.ServiceProviderSpeed(helper.findUser(id), doneServices.Count);
            speed = speed < 1 ? 1 : speed;
            List<ServicePath> paths = db.ServicePaths.Where(a => a.id.Equals(pathId)).ToList();
            if (paths.Count == 0)
            {
                paths = db.ServicePaths.Where(a => a.ServiceProviderId.Equals(null) && a.Enabled).ToList();
            }

            List<ServicePathViewModel> result = new List<ServicePathViewModel>();
            foreach (var item in paths)
            {
                int numOfOpenDreams = activeSerives.Where(a => a.ServicePathId.Equals(item.id)).Count();
                ServicePathViewModel temp = new ServicePathViewModel();
                temp.id = item.id;
                temp.Cost = item.Cost;
                temp.Name = item.Name;
                temp.NumberOfPeopleWaiting = numOfOpenDreams;
                temp.AvgWaitingTime = UserHelperLibrary.getWaitingTimeMessage(Double.Parse(speed.ToString()),
                Double.Parse(numOfOpenDreams.ToString())).Replace("Your average waiting time is ", "");
                result.Add(temp);
            }
            return result[0];
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("VerifyEmail")]
        public async Task<IHttpActionResult> VerifyEmail(VerifyEmail model)
        {
            var user = db.Users.Where(a => a.Email.Equals(model.email)).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("No matching user with this email!");
            }
            UserVerificationHelper.VerificationResult result = UserVerificationHelper.verifyCode(model.userId == null ? user.Id : model.userId, model.code);

            if (result.status.Equals("500"))
                return BadRequest(result.message);
            else
                return Ok(result);

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("RequestPasswordReset")]
        public async Task<IHttpActionResult> RequestPasswordReset([FromUri] String Email)
        {
            var user = db.Users.Where(a => a.Email.Equals(Email)).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("No matching user with this email!");
            }
            UserVerificationHelper.VerificationResult result = UserVerificationHelper.generateVerificationLog(user.Id, Email);

            if (result.status.Equals("500"))
                return BadRequest(result.message);
            else
                return Ok(result);

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ForgetPassword")]
        public async Task<IHttpActionResult> ForgetPassword([FromUri] String Email, [FromUri]String NewPassword)
        {
            var user = db.Users.Where(a => a.Email.Equals(Email)).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("No matching user with this email!");
            }

            IdentityResult remove = await UserManager.RemovePasswordAsync(user.Id);
            IdentityResult result = await UserManager.AddPasswordAsync(user.Id, NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();

        }

        // POST api/Account/updateUserInfo
        [Route("updateUserInfo")]
        public async Task<IHttpActionResult> updateUserInfo([FromBody]updateUserInfoBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = db.Users.AsNoTracking().Where(a => a.Id.Equals(model.id)).FirstOrDefault();
            if (user == null)
                await core.throwExcetpion("No matching user!");
            if (model.Age != null)
                user.Age = model.Age;
            if (model.Country != null)
                user.Country = model.Country;
            if (model.JobDescription != null)
                user.JobDescription = model.JobDescription;
            if (model.MartialStatus != null)
                user.MartialStatus = model.MartialStatus;
            if (model.Name != null)
                user.Name = model.Name;
            if (model.Sex != null)
                user.Sex = model.Sex;
            if (model.Status != null)
                user.Status = model.Status;
            if (model.PersonalDescription != null)
                user.PersonalDescription = model.PersonalDescription;
            if (model.PictureId != null)
                user.PictureId = model.PictureId;


            user.LastModificationDate = DateTime.Now;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return Ok(user);
        }
        #endregion
    }
}
