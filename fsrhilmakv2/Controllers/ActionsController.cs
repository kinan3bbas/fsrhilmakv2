﻿using fsrhilmakv2.Bindings;
using fsrhilmakv2.Extra;
using fsrhilmakv2.Models;
using fsrhilmakv2.ViewModels;
using Newtonsoft.Json.Linq;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

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
        private CompetitionLibrary libComp = new CompetitionLibrary();
        private JobLibrary jobLibrary = new JobLibrary();



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
            Service service = db.Services.Where(a => a.id.Equals(temp.id)).Include(a=>a.ServiceProvider)
                .Include(a=>a.UserWork)
                .FirstOrDefault();
            if (temp.Explanation.Equals("") || temp.Explanation == null)
            {
                core.throwExcetpion("Explanation can't be null");
            }

            service.Explanation = temp.Explanation;
            service.ExplanationDate = DateTime.Now;
            service.Status = CoreController.ServiceStatus.Done.ToString();
            PublicService publicService = db.PublicServices.Where(a => a.ServiceId.Equals(temp.id)).FirstOrDefault();
            if (publicService != null)
            {
                db.Entry(publicService).State = EntityState.Deleted;
            }
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
            if (!service.PrivateService) {
                PostToFacebook(service);
            }

            return service;
        }

        public void PostToFacebook(Service sr)
        {
            Facebookk fb = new Facebookk();
            String _hashtag = sr.UserWork.AdjectiveName;
            String hashtag = _hashtag.Replace(" ", "_");
            fb.PublishSimplePost("نص الخدمة: " + "\n" + sr.Description + "\n"
                + "التفسير:" + "\n" + sr.Explanation + "\n"
                + "المفسر: " + "  " + sr.ServiceProvider.Name + "\n"
                + "https://ahalzekr.com/Home/Service?idd=" + sr.id + "\n" +
                "يوفر موقع أهل الذكر الخدمات التالية: \n 1- تفسير الأحلام وتعبير الرؤى يمكن التواصل المباشر مع المختصين لتفسير حلمك وفك الرموز\n 2- الفتاوى الشرعية وفق الكتاب والسنة النبوية يمكن التواصل المباشر مع المفتين\n 3- رقية شرعية وفق الكتاب والسنة النبوية يمكن التواصل المباشر مع الرقاة الشرعيين\n قم بزيارة موقع أهل الذكر"
                + "\n https://ahalzekr.com/"
                 + "\n" + "بلغوا عني ولو بآية" 
                +"\n" + getRandomAyayAsync().Result+
                "\n #"+hashtag);
        }

        //[Route("GetRandom")]
        //[HttpGet]
        //[AllowAnonymous]
        public async Task<string> getRandomAyayAsync()
        {
            Random r = new Random();
            int x = r.Next(1, 6236);
            string result = "";

            String URL = "http://api.alquran.cloud/ayah/" + x;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            var responseTask = client.GetAsync("");
            responseTask.Wait();

            var response = responseTask.Result; // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                string responseBody =await response.Content.ReadAsStringAsync();
                JObject json =  JObject.Parse(responseBody);
                JObject json2= JObject.Parse(json["data"].ToString());
                result = json2["text"].ToString();

            }


            // Make any other calls using HttpClient here.

            // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();
            return result;
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


            Service service = db.Services.Where(a => a.id.Equals(temp.id))/*.Include(a=>a.Comments)*/
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


            //if (service.Comments.Count > 0)
            //{
            //    foreach (var item in service.Comments)
            //    {
            //        db.Entry(item).State = EntityState.Deleted;
            //    }
            //}

            PublicService publicService = db.PublicServices.Where(a => a.ServiceId.Equals(temp.id)).FirstOrDefault();
            if (publicService != null)
            {
                db.Entry(publicService).State = EntityState.Deleted;
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
            if (temp.Description!=null)
            {
                service.Description = temp.Description;
            }
            if (temp.Explanation!=null)
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
            service.numberOfViews = service.numberOfViews + 1;
            SaveService(service);
            return getMapping(service);

        }

        //****************************** Statistics*************************************

        [Route("getUsersStatistics")]
        [HttpGet]
        [AllowAnonymous]
        public Statistics getUsersStatistics()
        {
            //List<ApplicationUser> users = db.Users.Where(a => a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
            //List<ApplicationUser> Clients = users.Where(a => a.Type.Equals(CoreController.UserType.Client.ToString())).ToList();
            //List<ApplicationUser> ServiceProviders = users.Where(a => a.Type.Equals(CoreController.UserType.Service_Provider.ToString())&& a.verifiedInterpreter).ToList();
            //List<Service> AllServices = db.Services.ToList();


            //StatisticsViewModel result = new StatisticsViewModel();
            //result.AllClients = Clients.Count();

            //result.AllUsers = users.Count();
            //result.AllServiceProviders = ServiceProviders.Count();
            //result.AllActiveClients = result.AllClients - (int)(result.AllClients * 0.8);
            //result.AllActiveServices = AllServices.Where(a => a.Status.Equals(CoreController.ServiceStatus.Active.ToString())).Count();
            //result.AllDoneServices = AllServices.Where(a => a.Status.Equals(CoreController.ServiceStatus.Done.ToString())).Count();
            //result.AllServices = AllServices.Count();
            //Random random = new Random();
            //result.AllActiveClientsInThePastThreeDays = result.AllClients + (int)(result.AllClients * 0.7);

            //result.AllDreamUsers = helper.getServiceProviders(CoreController.UserWorkCode.Dream.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            //result.AllRouqiaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Rouqia.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            //result.AllIftaaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Iftaa.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            //result.AllIstasharaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Istishara.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            //result.AllMedicalUsers = helper.getServiceProviders(CoreController.UserWorkCode.Medical.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            //result.AllLawUsers = helper.getServiceProviders(CoreController.UserWorkCode.Law.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            //return result;
            return db.Statistics.OrderByDescending(a => a.CreationDate).FirstOrDefault();


        }

        //[Route("GenerateUserStatisticsJob")]
        //[AllowAnonymous]
        //[HttpGet]
        public void GenerateUserStatisticsJob()
        {

            List<ApplicationUser> users = db.Users.Where(a => a.Status.Equals(CoreController.UserStatus.Active.ToString())).ToList();
            List<ApplicationUser> Clients = users.Where(a => a.Type.Equals(CoreController.UserType.Client.ToString())).ToList();
            List<ApplicationUser> ServiceProviders = users.Where(a => a.Type.Equals(CoreController.UserType.Service_Provider.ToString()) && a.verifiedInterpreter).ToList();
            List<Service> AllServices = db.Services.ToList();

            List<Service> services = db.Services.Where(a => a.Status.Equals("Done")).ToList();
            Statistics result = new Statistics();
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
            result.CreationDate = DateTime.Now;
            result.LastModificationDate = DateTime.Now;
            result.AllDreams = services.Where(a => a.UserWorkId.Equals(26)).Count();
            result.AllRouqat = services.Where(a => a.UserWorkId.Equals(27)).Count();
            result.AllIftaa = services.Where(a => a.UserWorkId.Equals(28)).Count();
            result.AllIstashara = services.Where(a => a.UserWorkId.Equals(29)).Count();
            result.AllMedical = services.Where(a => a.UserWorkId.Equals(36)).Count();
            result.AllLaw = services.Where(a => a.UserWorkId.Equals(37)).Count();
            db.Statistics.Add(result);
            db.SaveChanges();
            //return Ok(result);
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
            service.PrivateService = temp.PrivateService;
            if (!temp.UseUserPoints)
            {
                AddPayment(temp, service);
                sendEmail2("New Payment was added, Client Name " + core.getCurrentUser().Name
                    + ", Service Id : " + service.id + ", Payment amount :" + temp.Amount + ", Payment Method: " + temp.Method,
                    "gerranzuv@gmail.com");
                sendEmail2("New Payment was added, Client Name " + core.getCurrentUser().Name
                    + ", Service Id : " + service.id + ", Payment amount :" + temp.Amount + ", Payment Method: " + temp.Method,
                    "hassan.hallak4@gmail.com");
            }
            else
            {

                //SystemParameter param = db.SystemParameters.Where(x => x.Code.Equals("ServicePricePerPoints")).AsNoTracking().FirstOrDefault();
                //int ServicePricePerPoints = Int32.Parse(param.Value);

                //if (service.Creator.PointsBalance - ServicePricePerPoints > 0)
                //{
                //    service.Creator.PointsBalance = service.Creator.PointsBalance - ServicePricePerPoints;
                //    //db.Entry(user).State = EntityState.Modified;
                //    // db.SaveChanges();
                //}
                if (service.Creator.PointsBalance - (temp.NumberOfPoints != null ? temp.NumberOfPoints : 5) > 0)
                {
                    service.Creator.PointsBalance = service.Creator.PointsBalance - (temp.NumberOfPoints != null ? temp.NumberOfPoints : 5);
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
        

        [Route("GetPublicServicesWithoutFilter")]
        [HttpGet]
        public List<PublicService> GetPublicServicesWithoutFilter()
        {

            String userId = core.getCurrentUser().Id;
            if (!core.getCurrentUser().verifiedInterpreter)
                return new List<PublicService>();
            ApplicationUser user = db.Users.Where(a => a.Id.Equals(userId)).Include(a => a.userWorkBinding).FirstOrDefault();
            List<int> userWorkIds = new List<int>();
            foreach (var item in user.userWorkBinding)
            {
                userWorkIds.Add(item.UserWorkId);
            }
            List<PublicService> publicServices = db.PublicServices.Where(a => userWorkIds.Contains(a.UserWorkId)).ToList();
            foreach (var item in publicServices)
            {
                item.id = item.ServiceId;
            }
            return publicServices;

        }

        [Route("GeneratePublicServicesJob")]
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GeneratePublicServicesJob()
        {

            String _hoursToPublicService = ParameterRepository.findByCode("hours_To_Public_Service");
            int hoursToPublicService = _hoursToPublicService == null ? 24 : (int)Int32.Parse(_hoursToPublicService);
            DateTime dateToCompare = DateTime.Now.AddHours(-1 * hoursToPublicService);
            //Service Provider Speed
            String _PublicServiceUserSpeed = ParameterRepository.findByCode("Public_Service_User_Speed");
            double PublicServiceUserSpeed = _PublicServiceUserSpeed == null ? 1.0 : Double.Parse(_PublicServiceUserSpeed);
            //Avg Service In a day
            String _PublicServiceUserAvg = ParameterRepository.findByCode("Public_Service_User_Avg_Services");
            double PublicServiceUserAvg = _PublicServiceUserAvg == null ? 1.0 : Double.Parse(_PublicServiceUserAvg);


            List<int> userWorkIds = new List<int>();
            List<Service> services = db.Services.Where(a => a.Status.Equals("Active") &&
                a.ServiceProviderNewDate.CompareTo(dateToCompare) <= 0 && a.PublicServiceAction)
                .Include("Comments")
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator")
                .OrderByDescending(a => a.CreationDate)
                .ToList();


            List<PublicService> result = new List<PublicService>();
            foreach (var item in services)
            {
                result.Add(getMappingPublicService(item));
            }


            result= result.Where(a => a.ServiceProviderAvgServices <= PublicServiceUserAvg && a.ServiceProviderSpeed <= PublicServiceUserSpeed).ToList();
            List<PublicService> publicServices = db.PublicServices.ToList();
            List<int> ids = publicServices.Select(a => a.ServiceId).ToList();
            foreach (var item in result)
            {
                if (ids.Contains(item.ServiceId))
                    continue;
                db.PublicServices.Add(item);
            }
            db.SaveChanges();
            return Ok();
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

            //List<Service> services = db.Services.Where(a => a.Status.Equals("Done")).ToList();
            Statistics temp = db.Statistics.OrderByDescending(a => a.CreationDate).FirstOrDefault();
            //temp.AllDreamUsers = helper.getServiceProviders(CoreController.UserWorkCode.Dream.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            //temp.AllRouqiaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Rouqia.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            //temp.AllIftaaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Iftaa.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            //temp.AllIstasharaUsers = helper.getServiceProviders(CoreController.UserWorkCode.Istishara.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            //temp.AllMedicalUsers = helper.getServiceProviders(CoreController.UserWorkCode.Medical.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            //temp.AllLawUsers = helper.getServiceProviders(CoreController.UserWorkCode.Law.ToString(), CoreController.UserStatus.Active.ToString()).Count();
            List<UserWork> UserWorks = db.UserWorks.Where(a => a.Enabled).ToList();
            //temp.AllDreams = services.Where(a => a.UserWorkId.Equals(26)).Count();
            //temp.AllRouqat = services.Where(a => a.UserWorkId.Equals(27)).Count();
            //temp.AllIftaa = services.Where(a => a.UserWorkId.Equals(28)).Count();
            //temp.AllIstashara = services.Where(a => a.UserWorkId.Equals(29)).Count();
            //temp.AllMedical = services.Where(a => a.UserWorkId.Equals(36)).Count();
            //temp.AllLaw = services.Where(a => a.UserWorkId.Equals(37)).Count();
            List<UserWorkViewModel> result = new List<UserWorkViewModel>();
            foreach (var item in UserWorks)
            {
                UserWorkViewModel u = new UserWorkViewModel();
                u.UserWork = item;

                if (item.Code.Equals(CoreController.UserWorkCode.Dream.ToString())) {
                    u.UserCount = temp.AllDreamUsers;
                    u.DoneServices = temp.AllDreams;
                }


                else if (item.Code.Equals(CoreController.UserWorkCode.Rouqia.ToString()))
                {
                    u.UserCount = temp.AllRouqiaUsers;
                    u.DoneServices = temp.AllRouqat;
                }

                else if (item.Code.Equals(CoreController.UserWorkCode.Iftaa.ToString()))
                {
                    u.UserCount = temp.AllIftaaUsers;
                    u.DoneServices = temp.AllIftaa;
                }

                else if (item.Code.Equals(CoreController.UserWorkCode.Istishara.ToString()))
                {
                    u.UserCount = temp.AllIstasharaUsers;
                    u.DoneServices = temp.AllIstashara;
                }

                else if (item.Code.Equals(CoreController.UserWorkCode.Medical.ToString()))
                {
                    u.UserCount = temp.AllMedicalUsers;
                    u.DoneServices = temp.AllMedical;
                }

                else if (item.Code.Equals(CoreController.UserWorkCode.Law.ToString()))
                {
                    u.UserCount = temp.AllLawUsers;
                    u.DoneServices = temp.AllLaw;
                }
                    
                result.Add(u);

            }
            return result;
        }
        //**************************** Like Service***********************************
        [Route("Like")]
        [HttpGet]
        public async Task<IHttpActionResult> LikeService([FromUri] int id)
        {
            Service service = db.Services.Find(id);
            service.numberOfLikes++;
            SaveService(service);
            return Ok(service);
        }

        //*************************** Competition List********************
        [Route("GetCompetitions")]
        public IHttpActionResult GetCompetitions([FromUri]String status, [FromUri] int skip, [FromUri] int top)
        {
            List<CompetitionViewModel> result = new List<CompetitionViewModel>();
            String userId = core.getCurrentUser().Id;
            if (!core.getCurrentUser().verifiedInterpreter)
                return Ok(result);
            ApplicationUser user = db.Users.Where(a => a.Id.Equals(userId)).Include(a => a.userWorkBinding).FirstOrDefault();
            List<int> userWorkIds = new List<int>();
            foreach (var item in user.userWorkBinding)
            {
                userWorkIds.Add(item.UserWorkId);
            }
            skip = skip == null ? 0 : skip;
            top = top == null ? 5 : top;
            
            List<Competition> competitions= db.Competitions.Where(a => a.Status.Equals(status)
            && userWorkIds.Contains(a.UserWorkId))
            .Include(a=>a.UserWork)
            .Include(a=>a.prize)
            .ToList();
            int count = competitions.Count();
            foreach (var item in competitions)
            {
                result.Add(GetCompetitionMapping(item));
            }
            var genericResutl = new { Competitions = result.OrderBy(a => a.EndDate).Skip(skip).Take(top), Count = count };
            return Ok(genericResutl);
        }

        //*************************** Get Competition Results********************
        [AllowAnonymous]
        [Route("GetCompetitionResult")]
        public IHttpActionResult GetCompetitionResult(int CompetitionId)
        {

            List<CompetitionResult> resutl = new List<CompetitionResult>();
            Competition Competition = db.Competitions.Where(a=>a.id.Equals(CompetitionId))
                .Include(a=>a.prize)
                .Include(a => a.UserWork)
                .FirstOrDefault();
            if (Competition.Status.Equals(CoreController.CompetitionStatus.Active.ToString()))
            {
                //temp results here
                List<UserWorkBinding> bindings = db.UserWorkBindings.Where(a => a.UserWorkId == Competition.UserWorkId && a.User.Status != "Deleted"
                     && a.User.Type == "Service_Provider"
                    && a.User.verifiedInterpreter).Include("User").ToList();
                List<ApplicationUser> users = bindings.Select(a => a.User).ToList();
                //FinishCompetitionJob();
                return Ok(libComp.getFinalList(Competition, users, Competition.StartDate.Value, true).OrderBy(a=>a.rank));
            }
            else if (Competition.Status.Equals(CoreController.CompetitionStatus.Finished.ToString())) {
                //Final Results here 
                return Ok(db.CompetitionResults.Where(a => a.competitionId.Equals(CompetitionId)).OrderBy(a => a.rank).Include(a => a.ServiceProvider).ToList());
            }

            
            return Ok(resutl);
        }

        //[AllowAnonymous]
        //[HttpGet]
        //[Route("FinishCompetitionJob")]
        public void FinishCompetitionJob()
        {
            DateTime now = DateTime.Now.ToUniversalTime().AddHours(3);
            List<Competition> Competitions = db.Competitions.Where(a => a.Status.Equals("Active") && a.EndDate.CompareTo(DateTime.Now) <= 0)
                .Include(a => a.prize)
                .Include(a => a.UserWork).ToList();
            foreach (var Competition in Competitions)
            {
                List<UserWorkBinding> bindings = db.UserWorkBindings.Where(a => a.UserWorkId == Competition.UserWorkId && a.User.Status != "Deleted"
                     && a.User.Type == "Service_Provider"
                    && a.User.verifiedInterpreter).Include("User").ToList();
                List<ApplicationUser> users = bindings.Select(a => a.User).ToList();
                libComp.finishCompetition(Competition, libComp.getFinalList(Competition, users, Competition.StartDate.Value, false));

            }

            //return Ok();
        }
        public void SaveService(Service Service)
        {
            Service.LastModificationDate = DateTime.Now;
            //Service.ModifierId = core.getCurrentUser().Id;
            db.Entry(Service).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public CompetitionViewModel GetCompetitionMapping(Competition comp)
        {
            List<double> prizes = new List<double> { comp.prize.rank1, comp.prize.rank2, comp.prize.rank3, comp.prize.rank4, comp.prize.rank5, comp.prize.rank6, comp.prize.rank7,
            comp.prize.rank8,comp.prize.rank9,comp.prize.rank10,comp.prize.rank11,comp.prize.rank12,comp.prize.rank13,comp.prize.rank14,comp.prize.rank15,comp.prize.rank16,comp.prize.rank17,
            comp.prize.rank18,comp.prize.rank19,comp.prize.rank20,comp.prize.rank21,comp.prize.rank22,comp.prize.rank23,comp.prize.rank24,comp.prize.rank25,
            comp.prize.rank26,comp.prize.rank27,comp.prize.rank28,comp.prize.rank29,comp.prize.rank30,comp.prize.rank31,
            comp.prize.rank32,comp.prize.rank33,comp.prize.rank34,comp.prize.rank35,comp.prize.rank36,comp.prize.rank37,comp.prize.rank38,comp.prize.rank39,comp.prize.rank40,comp.prize.rank41,comp.prize.rank42,comp.prize.rank43,
            comp.prize.rank44,comp.prize.rank45,comp.prize.rank46,comp.prize.rank47,comp.prize.rank48,comp.prize.rank49,comp.prize.rank50,
            comp.prize.rank51, comp.prize.rank52, comp.prize.rank53, comp.prize.rank54, comp.prize.rank55, comp.prize.rank56, comp.prize.rank57,comp.prize.rank58,comp.prize.rank59,comp.prize.rank60
            ,comp.prize.rank61, comp.prize.rank62, comp.prize.rank63, comp.prize.rank64, comp.prize.rank65, comp.prize.rank66, comp.prize.rank67,
            comp.prize.rank68,comp.prize.rank69,comp.prize.rank70,

            comp.prize.rank71, comp.prize.rank72, comp.prize.rank73, comp.prize.rank74, comp.prize.rank75, comp.prize.rank76, comp.prize.rank77,
            comp.prize.rank78,comp.prize.rank79,comp.prize.rank80,

            comp.prize.rank81, comp.prize.rank82, comp.prize.rank83, comp.prize.rank84, comp.prize.rank85, comp.prize.rank86, comp.prize.rank87,
            comp.prize.rank88,comp.prize.rank89,comp.prize.rank90,

            comp.prize.rank91, comp.prize.rank92, comp.prize.rank93, comp.prize.rank94, comp.prize.rank95, comp.prize.rank96, comp.prize.rank97,
            comp.prize.rank98,comp.prize.rank99,comp.prize.rank100 }

            ;
            CompetitionViewModel temp = new CompetitionViewModel();
            temp.duration = comp.duration;
            temp.EndDate = comp.EndDate.ToUniversalTime() ;
            temp.StartDate = comp.StartDate.Value.ToUniversalTime();
            temp.prizes = prizes;
            temp.Goal = comp.Goal;
            temp.repeat = comp.repeat;
            temp.Status = comp.Status;
            temp.UserWork = comp.UserWork;
            temp.Name = comp.Name;
            temp.id = comp.id;
            return temp;


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
            result.id = service.id;
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

        public PublicService getMappingPublicService(Service service)
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
            PublicService result = new PublicService();
            //result.Comments = service.Comments;
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
            result.ServiceId = service.id;
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
            result.id = service.id;
            return result;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("JobRunner")]
        public IHttpActionResult JobRunner()
        {
            List<ScheduledJob> jobs = db.ScheduledJobs.Where(a => a.Status.Equals("Active")).ToList();
            DateTime dateForLogs = DateTime.Now.AddDays(-2);
            List<ScheduledJobLog> logs = db.ScheduledJobLogs.Where(a => a.CreationDate.CompareTo(dateForLogs) >= 0).ToList();
            foreach (var item in jobs)
            {
                ScheduledJobLog log = logs.Where(a => a.scheduledJobId == item.id)
                    .OrderByDescending(a => a.CreationDate)
                    .FirstOrDefault();
                if (log ==null ||checkIfJobShouldRun(item, log))
                {

                        try
                        {
                            //call the required function
                            jobRunner(item);
                            //add new log
                            db.ScheduledJobLogs.Add(addNewLog(item));
                            //sendEmail(item.Code + DateTime.Now.ToString() +" The job is running","gerranzuv@gmail.com" );
                        }
                        catch (Exception e)
                        {
                            sendEmail(item.Code+ DateTime.Now.ToString() +  e.StackTrace, "gerranzuv@gmail.com");

                            //add new log
                            db.ScheduledJobLogs.Add(addNewLog(item, e.Message));
                            db.SaveChanges();
                        }
     
                   
                    
                }
                else
                {
                    continue;
                }

            }
            
            db.SaveChanges();
            return Ok();
        }

        private bool checkIfJobShouldRun(ScheduledJob job,ScheduledJobLog lastLog)
        {
            LocalDateTime d1 = new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            LocalDateTime d2 = new LocalDateTime(lastLog.CreationDate.Year, lastLog.CreationDate.Month, lastLog.CreationDate.Day, lastLog.CreationDate.Hour, lastLog.CreationDate.Minute, lastLog.CreationDate.Second);
            long minutes = Period.Between(d2, d1, PeriodUnits.Minutes).Minutes;

            if (job.interval <= minutes)
                return true;
            return false;
        }

        private ScheduledJobLog addNewLog(ScheduledJob job)
        {
            ScheduledJobLog log = new ScheduledJobLog();
            log.CreationDate = DateTime.Now;
            log.LastModificationDate = DateTime.Now;
            log.scheduledJobId = job.id;
            return log;
        }
        private ScheduledJobLog addNewLog(ScheduledJob job,string error)
        {
            ScheduledJobLog log = new ScheduledJobLog();
            log.CreationDate = DateTime.Now;
            log.LastModificationDate = DateTime.Now;
            log.error = error;
            log.scheduledJobId = job.id;
            return log;
        }

        private void jobRunner(ScheduledJob job)
        {
            if(job.Code.Equals("UserInfoCashJob"))
                jobLibrary.GenenrateUserInfoCashJob();
            //else if (job.Code.Equals("FinishCompetition"))
            //    FinishCompetitionJob();
            else if (job.Code.Equals("GenerateUserStatistics"))
                GenerateUserStatisticsJob();
            //else if (job.Code.Equals("GeneratePublicServices"))
            //    GeneratePublicServicesJob();

        }

        private bool sendEmail(String code, String email)
        {

            
                String subject = "Error";
                String body = code;
                List<string> receivers = new List<string>();
                receivers.Add(email);
                EmailHelper.sendEmail(receivers, subject, body);
                return true;
            
        }

        private bool sendEmail2(String code, String email)
        {


            String subject = "Payment Notice";
            String body = code;
            List<string> receivers = new List<string>();
            receivers.Add(email);
            EmailHelper.sendEmail(receivers, subject, body);
            return true;

        }


        [AllowAnonymous]
        [HttpGet]
        [Route("RunUserCashJob")]
        public IHttpActionResult RunUserCashJob()
        {


            jobLibrary.GenenrateUserInfoCashJob();
            db.SaveChanges();
            return Ok();
        }

        [Route("GetQuran")]
        [HttpGet]
        [AllowAnonymous]
        public List<Quran> GetQuran()
        {
            return (db.Qurans.ToList()); 

        }
    }
}