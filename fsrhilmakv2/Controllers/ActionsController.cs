using fsrhilmakv2.Bindings;
using fsrhilmakv2.Extra;
using fsrhilmakv2.Models;
using fsrhilmakv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace fsrhilmakv2.Controllers
{
    [Authorize]
    [RoutePrefix("api/actions")]
    public class ActionsController : ApiController
    {
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();
        private UserHelperLibrary helper = new UserHelperLibrary();



        //****************************** Service Explanation*************************************
        // POST api/ActionsController/AddExplanation
        [Route("AddExplanation")]
        [HttpPost]
        public Service AddExplanation([FromBody] Service temp)
        {
            if (temp.id.Equals(null))
            {
                core.throwExcetpion("Id is null");
            }
            Service service = db.Services.Where(a => a.id.Equals(temp.id))
                .FirstOrDefault();
            if (temp.Explanation.Equals("") || temp.Explanation == null)
            {
                core.throwExcetpion("Explanation can't be null");
            }

            service.Explanation = temp.Explanation;
            service.ExplanationDate = DateTime.Now;
            service.Status = CoreController.ServiceStatus.Done.ToString();
            //if (temp.ServiceProviderId != null)
            //{
            //    DreamHistory history = new DreamHistory();
            //    history.NewInterpreterId = temp.ServiceProviderId;
            //    history.OldInterpreterId = service.ServiceProviderId;
            //    history.ServiceId = service.id;
            //    history.CreatorId = core.getCurrentUser().Id;
            //    history.CreationDate = DateTime.Now;
            //    history.LastModificationDate = DateTime.Now;
            //    service.ServiceProviderNewDate = DateTime.Now;
            //    db.DreamHistories.Add(history);
            //    service.ServiceProviderId = temp.ServiceProviderId;
            //}

            SaveService(service);
            return service;
        }

        //****************************** Service Receive *************************************
        // POST api/ActionsController/ReceiveService
        [Route("ReceiveService")]
        [HttpPost]
        public Service ReceiveService([FromBody] Service temp)
        {
            
            if (temp.id.Equals(null))
            {
                core.throwExcetpion("Id is null");
            }

            if (temp.ServiceProviderId.Equals(null))
            {
                core.throwExcetpion("Service Provider id can't be null");
            }

            
            Service service = db.Services.Where(a => a.id.Equals(temp.id))
                .FirstOrDefault();

            String _hoursToPublicService = ParameterRepository.findByCode("hours_To_Public_Service");
            int hoursToPublicService = (int)Int32.Parse(_hoursToPublicService);
            DateTime dateToCompare = DateTime.Now.AddHours(-1 * hoursToPublicService);

            if (service.ServiceProviderNewDate.CompareTo(dateToCompare) > 0)
            {
                core.throwExcetpion("لقد تم استلام الخدمة من قبل مستخدم آخر");
            }

            DreamHistory history = new DreamHistory();
            history.NewInterpreterId = temp.ServiceProviderId;
            history.OldInterpreterId = service.ServiceProviderId;
            history.ServiceId = service.id;
            history.CreatorId = core.getCurrentUser().Id;
            history.CreationDate = DateTime.Now;
            history.LastModificationDate = DateTime.Now;
            db.DreamHistories.Add(history);
            service.ServiceProviderId = temp.ServiceProviderId;
            service.ServiceProviderNewDate = DateTime.Now;



            SaveService(service);
            return service;
        }

        //****************************** User Rating*************************************

        // POST api/ActionsController/AddRating
        [Route("AddRating")]
        [HttpPost]
        public Service AddRating([FromBody] Service temp)
        {
            if (temp.id.Equals(null))
            {
                core.throwExcetpion("Id is null");
            }
            Service service = db.Services.Where(a => a.id.Equals(temp.id))
                .FirstOrDefault();
            if (!service.Status.Equals("Done"))
                core.throwExcetpion("User can't rate interpreter until he explain the dream!");
            if (temp.UserRating < 0 || temp.UserRating > 5)
            {
                core.throwExcetpion("Rating can only be between 0 and 5");
            }
            service.RatingMessage = temp.RatingMessage;
            service.UserRating = temp.UserRating;
            service.RatingDate = DateTime.Now;

            SaveService(service);
            return service;
        }
        //****************************** Service Edit Text*************************************
        // POST api/ActionsController/EditService
        [Route("EditService")]
        [HttpPost]
        public Service EditService([FromBody] Service temp)
        {
            if (temp.id.Equals(null))
            {
                core.throwExcetpion("Id is null");
            }
            Service service = db.Services.Where(a => a.id.Equals(temp.id))
                .FirstOrDefault();
            if (!temp.Description.Equals(null))
            {
                service.Description = temp.Description;
            }
            if (!temp.Explanation.Equals(null))
            {
                service.Explanation = temp.Explanation;
            }

            SaveService(service);
            return service;
        }

        //****************************** Single Service Info*************************************

        [Route("GetSingleServiceInfo")]
        [HttpGet]
        [AllowAnonymous]
        public ServiceViewModel GetSingleServiceInfo([FromUri] int id)
        {
            if (id.Equals(null))
            {
                core.throwExcetpion("Id is null");
            }
            Service service = db.Services.Where(a => a.id.Equals(id))
                .Include("Comments")
                .Include("ServicePath")
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator")
                .FirstOrDefault();
            return getMapping(service);

        }

        //****************************** Statistics*************************************

        [Route("getUsersStatistics")]
        [HttpGet]
        [AllowAnonymous]
        public StatisticsViewModel getUsersStatistics()
        {
            List<ApplicationUser> users = db.Users.Where(a => a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
            List<ApplicationUser> Clients = users.Where(a => a.Type.Equals(CoreController.UserType.Client.ToString())).ToList();
            List<ApplicationUser> ServiceProviders = users.Where(a => a.Type.Equals(CoreController.UserType.Service_Provider.ToString())).ToList();
            List<Service> AllServices = db.Services.ToList();


            StatisticsViewModel result = new StatisticsViewModel();
            result.AllClients = Clients.Count();

            result.AllUsers = users.Count();
            result.AllServiceProviders = ServiceProviders.Count();
            result.AllActiveClients = result.AllClients - (int)(result.AllClients * 0.8);
            result.AllActiveServices = AllServices.Where(a => a.Status.Equals(CoreController.ServiceStatus.Active.ToString())).Count();
            result.AllDoneServices = AllServices.Where(a => a.Status.Equals(CoreController.ServiceStatus.Done.ToString())).Count();
            result.AllServices = AllServices.Count();
            Random random = new Random();
            result.AllActiveClientsInThePastThreeDays = result.AllClients + (int)(result.AllClients * 0.7);

            result.AllDreamUsers = helper.getServiceProviders(CoreController.UserWorkCode.Dream.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllRouqiaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Rouqia.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllIftaaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Iftaa.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllIstasharaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Istishara.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllMedicalUsers = helper.getServiceProviders(CoreController.UserWorkCode.Medical.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllLawUsers = helper.getServiceProviders(CoreController.UserWorkCode.Law.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            return result;

        }

        //****************************** Change Plan *************************************
        // POST api/ActionsController/ChangePlan
        [Route("ChangePlan")]
        [HttpPost]
        public Service ChangePlan([FromBody] PaymentBinding temp)
        {

            Service service = db.Services.Where(a => a.id.Equals(temp.ServiceId)).Include("Creator")
                .FirstOrDefault();

            if (temp.Amount.Equals("") || temp.Amount == null)
            {
                core.throwExcetpion("Amount can't be null");
            }

            service.ServicePathId = temp.ServicePathId;
            if (!temp.UseUserPoints)
            {
                AddPayment(temp, service);
            }
            else
            {

                SystemParameter param = db.SystemParameters.Where(x => x.Code.Equals("ServicePricePerPoints")).AsNoTracking().FirstOrDefault();
                int ServicePricePerPoints = Int32.Parse(param.Value);

                if (service.Creator.PointsBalance - ServicePricePerPoints > 0)
                {
                    service.Creator.PointsBalance = service.Creator.PointsBalance - ServicePricePerPoints;
                    //db.Entry(user).State = EntityState.Modified;
                    // db.SaveChanges();
                }
                else
                {
                    core.throwExcetpion("No Remaining Points!");
                }

            }

            SaveService(service);
            return service;
        }


        private void AddPayment(PaymentBinding temp, Service service)
        {
            Payment payment = new Payment();
            payment.Method = temp.Method;
            payment.ServiceId = temp.ServiceId;
            payment.Status = "Done";
            payment.Currency = temp.Currency;
            payment.Amount = temp.Amount;
            //payment.ServiceProviderId = service.ServiceProviderId;
            payment.CreatorId = core.getCurrentUser().Id;
            payment.ModifierId = core.getCurrentUser().Id;
            payment.CreationDate = DateTime.Now;
            payment.LastModificationDate = DateTime.Now;
            db.Payments.Add(payment);
        }

        //****************************** Get Public Services **************************
        [Route("GetPublicServices")]
        [HttpGet]
        [AllowAnonymous]
        public List<Service> GetPublicServices([FromUri] int? UserWorkId)
        {
            //Hours to public Serive
            String _hoursToPublicService = ParameterRepository.findByCode("hours_To_Public_Service");
            int hoursToPublicService = (int)Int32.Parse(_hoursToPublicService);
            DateTime dateToCompare = DateTime.Now.AddHours(-1 * hoursToPublicService);

            //Service Provider Speed
            String _PublicServiceUserSpeed = ParameterRepository.findByCode("Public_Service_User_Speed");
            double PublicServiceUserSpeed = Double.Parse(_PublicServiceUserSpeed);
            //Avg Service In a day
            String _PublicServiceUserAvg = ParameterRepository.findByCode("Public_Service_User_Avg_Services");
            double PublicServiceUserAvg = Double.Parse(_PublicServiceUserAvg);

            List<Service> services = db.Services.Where(a => a.Status.Equals("Active") &&
                a.ServiceProviderNewDate.CompareTo(dateToCompare) <= 0)
                .Include("Comments")
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator")
                .OrderByDescending(a => a.CreationDate)
                .ToList();
            if (UserWorkId != null)
            {
                services = services.Where(a => a.UserWorkId.Equals(UserWorkId)).ToList();
            }
            return services;

        }

        [Route("GetPublicServicesWithoutFilter")]
        [HttpGet]
        [AllowAnonymous]
        public List<ServiceViewModel> GetPublicServicesWithoutFilter()
        {

            String _hoursToPublicService = ParameterRepository.findByCode("hours_To_Public_Service");
            int hoursToPublicService = _hoursToPublicService==null? 24: (int)Int32.Parse(_hoursToPublicService);
            DateTime dateToCompare = DateTime.Now.AddHours(-1 * hoursToPublicService);
            //Service Provider Speed
            String _PublicServiceUserSpeed = ParameterRepository.findByCode("Public_Service_User_Speed");
            double PublicServiceUserSpeed = _PublicServiceUserSpeed==null?1.0:Double.Parse(_PublicServiceUserSpeed);
            //Avg Service In a day
            String _PublicServiceUserAvg = ParameterRepository.findByCode("Public_Service_User_Avg_Services");
            double PublicServiceUserAvg = _PublicServiceUserAvg==null?1.0: Double.Parse(_PublicServiceUserAvg);

            String userId = core.getCurrentUser().Id;
            ApplicationUser user = db.Users.Where(a => a.Id.Equals(userId)).Include("userWorkBinding").FirstOrDefault();
            List<int> userWorkIds = new List<int>();
            List<Service> services = db.Services.Where(a => a.Status.Equals("Active") &&
                a.ServiceProviderNewDate.CompareTo(dateToCompare) <= 0&&a.PublicServiceAction)
                .Include("Comments")
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator")
                .OrderByDescending(a => a.CreationDate)
                .ToList();

            if (user.userWorkBinding.Count > 0)
            {
                foreach (var item in user.userWorkBinding)
                {
                    userWorkIds.Add(item.UserWorkId);
                }
                services = services.Where(a => userWorkIds.Contains(a.UserWorkId)).ToList();
            }
            else
            {
                services = new List<Service>();
            }

            List<ServiceViewModel> result = new List<ServiceViewModel>();
            foreach (var item in services)
            {
                result.Add(getMapping(item));
            }


            return result.Where(a => a.ServiceProviderAvgServices < PublicServiceUserAvg && a.ServiceProviderSpeed <PublicServiceUserSpeed).ToList();


        }

        //****************************** Get Payments **************************
        [Route("GetPayments")]
        [HttpGet]
        public List<Payment> GetPayments()
        {
            ApplicationUser user = core.getCurrentUser();
            if (user.Type == "Client")
            {
                return db.Payments.Where(a => a.CreatorId.Equals(user.Id)).OrderByDescending(a => a.CreationDate).ToList();
            }
            else
            {
                return db.Payments.Where(a => a.Service.ServiceProviderId.Equals(user.Id)).OrderByDescending(a => a.CreationDate).ToList();
            }


        }

        [AllowAnonymous]
        [Route("GetUserBalance")]
        [HttpGet]
        public UserBalance GetUserBalance(String UserId)
        {

            return helper.getUserBalance(db.Users.Find(UserId));

        }

        [AllowAnonymous]
        [Route("GetUserSpeed")]
        [HttpGet]
        public double GetUserSpeed(String UserId)
        {


            List<Service> services = helper.getUserServices(UserId);
            List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
            List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
            double speed = UserHelperLibrary.ServiceProviderSpeed(helper.findUser(UserId), doneServices.Count);
            return speed;

        }
        //****************************** Functions*************************************

        [Route("AddBindings")]
        [HttpPost]
        [AllowAnonymous]
        public List<UserWorkBinding> AddBindings(List<UserWorkBinding> bindings)
        {
            foreach (var item in bindings)
            {
                item.CreationDate = DateTime.Now;
                item.LastModificationDate = DateTime.Now;
                db.UserWorkBindings.Add(item);
            }

            db.SaveChanges();
            return bindings;
        }

        //**************************** Get User Works***********************************
        [Route("GetUserWorks")]
        [HttpGet]
        [AllowAnonymous]
        public List<UserWorkViewModel> GetUserWorks()
        {


            StatisticsViewModel temp = new StatisticsViewModel();
            temp.AllDreamUsers = helper.getServiceProviders(CoreController.UserWorkCode.Dream.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            temp.AllRouqiaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Rouqia.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            temp.AllIftaaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Iftaa.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            temp.AllIstasharaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Istishara.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            temp.AllMedicalUsers = helper.getServiceProviders(CoreController.UserWorkCode.Medical.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            temp.AllLawUsers = helper.getServiceProviders(CoreController.UserWorkCode.Law.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            List<UserWork> UserWorks = db.UserWorks.Where(a => a.Enabled).ToList();
            List<UserWorkViewModel> result = new List<UserWorkViewModel>();
            foreach (var item in UserWorks)
            {
                UserWorkViewModel u = new UserWorkViewModel();
                u.UserWork = item;

                if (item.Code.Equals(CoreController.UserWorkCode.Dream.ToString()))
                    u.UserCount = temp.AllDreamUsers;

                else if (item.Code.Equals(CoreController.UserWorkCode.Rouqia.ToString()))
                    u.UserCount = temp.AllRouqiaUsers;
                else if (item.Code.Equals(CoreController.UserWorkCode.Iftaa.ToString()))
                    u.UserCount = temp.AllIftaaUsers;
                else if (item.Code.Equals(CoreController.UserWorkCode.Istishara.ToString()))
                    u.UserCount = temp.AllIstasharaUsers;
                else if (item.Code.Equals(CoreController.UserWorkCode.Medical.ToString()))
                    u.UserCount = temp.AllMedicalUsers;
                else if (item.Code.Equals(CoreController.UserWorkCode.Law.ToString()))
                    u.UserCount = temp.AllLawUsers;
                result.Add(u);

            }
            return result;
        }
        public void SaveService(Service Service)
        {
            Service.LastModificationDate = DateTime.Now;
            //Service.ModifierId = core.getCurrentUser().Id;
            db.Entry(Service).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }



        public ServiceViewModel getMapping(Service service)
        {
            List<Service> allService = db.Services.Where(a => a.ServiceProviderId.Equals(service.ServiceProviderId)
                && a.ServicePathId.Equals(service.ServicePathId)
                && a.Status.Equals(CoreController.ServiceStatus.Active.ToString())).ToList();

            List<Service> services = helper.getUserServices(service.ServiceProviderId);
            List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
            List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
            double speed = UserHelperLibrary.ServiceProviderSpeed(helper.findUser(service.ServiceProviderId), doneServices.Count);
            speed = speed < 1 ? 1 : speed;
            double avg = UserHelperLibrary.ServiceProviderAvgServices(helper.findUser(service.ServiceProviderId), services.Count);
            List<UsersDeviceTokens> Clienttokens = db.UsersDeviceTokens.Where(a => a.UserId.Equals(service.CreatorId)).ToList();
            List<UsersDeviceTokens> ServiceProvidertokens = db.UsersDeviceTokens.Where(a => a.UserId.Equals(service.ServiceProviderId)).ToList();

            AccountController accountCont = new AccountController();
            ServiceViewModel result = new ServiceViewModel();
            result.Comments = service.Comments;
            result.Country = service.Country;
            result.CreationDate = service.CreationDate;
            result.Creator = service.Creator;
            result.Description = service.Description;
            result.CreatorId = service.CreatorId;
            result.DidYouExorcism = service.DidYouExorcism;
            result.DreamDate = service.DreamDate;
            result.Explanation = service.Explanation;
            result.ExplanationDate = service.ExplanationDate;
            result.HaveYouPrayedBeforeTheDream = service.HaveYouPrayedBeforeTheDream;
            result.Id = service.id;
            result.IsThereWakefulness = service.IsThereWakefulness;
            result.JobStatus = service.JobStatus;
            result.KidsStatus = service.KidsStatus;
            result.LastModificationDate = service.LastModificationDate;
            result.Modifier = service.Modifier;
            result.ModifierId = service.ModifierId;
            result.Name = service.Name;
            result.numberOfLikes = service.numberOfLikes;
            result.numberOfViews = service.numberOfViews;
            result.PrivateService = service.PrivateService;
            result.PrivateServicePrice = service.PrivateServicePrice;
            result.PublicServiceAction = service.PublicServiceAction;
            result.RegligionStatus = service.RegligionStatus;
            result.ServicePathId = service.ServicePathId;
            result.ServiceProvider = service.ServiceProvider;
            result.ServiceProviderId = service.ServiceProviderId;
            result.Sex = service.Sex;
            result.SocialStatus = service.SocialStatus;
            result.Status = service.Status;
            result.UserWork = service.UserWork;
            result.UserWorkId = service.UserWorkId;
            result.ServicePath = service.ServicePath;
            result.NumberOfAllPeopleWaiting = allService.Count > 0 ? allService.Count : 0;
            result.NumberOfRemainingPeople = allService.Count > 0 ? allService.Where(a => a.CreationDate.CompareTo(service.CreationDate) < 0).Count() : 0;
            result.AvgWaitingTime = UserHelperLibrary.getWaitingTimeMessage(Double.Parse(speed.ToString()),
                Double.Parse(result.NumberOfRemainingPeople.ToString())).Replace("Your average waiting time is ", "");
            result.ClientToken = Clienttokens.Count > 0 ? Clienttokens[0].token : "";
            result.ServiceProviderToken = ServiceProvidertokens.Count > 0 ? ServiceProvidertokens[0].token : "";
            result.ServiceProviderSpeed = speed;
            result.ServiceProviderAvgServices = avg == 0 ? 1 : avg;

            return result;
        }
    }
}