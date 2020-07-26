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
            if (temp.ServiceProviderId != null)
            {
                DreamHistory history = new DreamHistory();
                history.NewInterpreterId = temp.ServiceProviderId;
                history.OldInterpreterId = service.ServiceProviderId;
                history.ServiceId = service.id;
                history.CreatorId = core.getCurrentUser().Id;
                history.CreationDate = DateTime.Now;
                history.LastModificationDate = DateTime.Now;
                db.DreamHistories.Add(history);
                service.ServiceProviderId = temp.ServiceProviderId;
            }
           
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
            List<ApplicationUser> users = db.Users.Where(a=>!a.Status.Equals(CoreController.UserStatus.Deleted.ToString())).ToList();
            List<ApplicationUser> Clients = db.Users.Where(a => a.Type.Equals(CoreController.UserType.Client.ToString())).ToList();
            List<ApplicationUser> ServiceProviders = db.Users.Where(a => a.Type.Equals(CoreController.UserType.Service_Provider.ToString())).ToList();
            List<Service> AllServices = db.Services.ToList();
            

            StatisticsViewModel result = new StatisticsViewModel();
            result.AllClients = Clients.Count();
            result.AllActiveClients = Clients.Where(a=>a.Online).Count();
            result.AllUsers = users.Count();
            result.AllServiceProviders = ServiceProviders.Count();
            result.AllActiveServices = AllServices.Where(a=>a.Status.Equals(CoreController.ServiceStatus.Active)).Count();
            result.AllDoneServices= AllServices.Where(a => a.Status.Equals(CoreController.ServiceStatus.Done)).Count();
            result.AllServices = AllServices.Count();
            Random random = new Random();
            result.AllActiveClientsInThePastThreeDays = result.AllActiveClients + random.Next(1, 50);

            result.AllDreamUsers = helper.getServiceProviders(CoreController.UserWorkCode.Dream.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllRouqiaUsers= helper.getServiceProviders(CoreController.UserWorkCode.Rouqia.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllIftaaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Iftaa.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllIstasharaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Istishara.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            result.AllMedicalUsers = helper.getServiceProviders(CoreController.UserWorkCode.Medical.ToString(), CoreController.UserStatus.Active.ToString()).Count();

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
                AddPayment(temp,service);
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

        private void AddPayment(PaymentBinding temp,Service service)
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
        public List<Service> GetPublicServices([FromUri] String Status,int? UserWorkId)
        {
            String _hoursToPublicService = ParameterRepository.findByCode("hours_To_Public_Service");
            int hoursToPublicService = (int)Int32.Parse(_hoursToPublicService);
            DateTime dateToCompare = DateTime.Now.AddHours(-1 * hoursToPublicService);
            List<Service> services = db.Services.Where(a => a.Status.Equals(Status) &&
                a.CreationDate.CompareTo(dateToCompare) <= 0)
                .Include("Comments")
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator")
                .OrderByDescending(a=>a.CreationDate)
                .ToList();
            if (UserWorkId != null)
            {
                services = services.Where(a => a.UserWorkId.Equals(UserWorkId)).ToList();
            }
            return services;

        }

        //****************************** Get Payments **************************
        [Route("GetPayments")]
        [HttpGet]
        [AllowAnonymous]
        public List<Payment> GetPayments([FromUri] String UserID)
        {
            ApplicationUser user = db.Users.Where(a => a.Id == UserID).FirstOrDefault();
            if (user.Type=="Client")
            {
               return db.Payments.Where(a => a.Creator.Equals(UserID)).OrderByDescending(a => a.CreationDate).ToList();
            }
            else
            {
                return db.Payments.Where(a => a.Service.ServiceProviderId.Equals(UserID)).OrderByDescending(a => a.CreationDate).ToList();
            }


        }
        //****************************** Functions*************************************

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
            result.AvgWaitingTime= UserHelperLibrary.getWaitingTimeMessage(Double.Parse(speed.ToString()),
                Double.Parse(result.NumberOfRemainingPeople.ToString())).Replace("Your average waiting time is ", "");




            return result;
        }
    }
}