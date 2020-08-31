using ControlPanel.Controllers;
using ControlPanel.Models;
using ControlPanel.ViewModels;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Extra
{
    public class UserHelperLibrary
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();

        public static double ServiceProviderSpeed(ApplicationUser user, int totalDreams)
        {
            //TimeSpan difference = DateTime.Now - user.CreationDate;
            TimeSpan span1 = new TimeSpan(DateTime.Now.Ticks);
            //TimeSpan span2 = new TimeSpan(user.CreationDate.Ticks);
            LocalDateTime d1 = new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            LocalDateTime d2 = new LocalDateTime(user.CreationDate.Year, user.CreationDate.Month, user.CreationDate.Day, user.CreationDate.Hour, user.CreationDate.Minute, user.CreationDate.Second);
            long days = Period.Between(d2, d1).Days;

            return days == 0 ? totalDreams : totalDreams / days;
        }

        public static double ServiceProviderAvgServices(ApplicationUser user, int totalDreams)
        {
            //TimeSpan difference = DateTime.Now - user.CreationDate;
            TimeSpan span1 = new TimeSpan(DateTime.Now.Ticks);
            //TimeSpan span2 = new TimeSpan(user.CreationDate.Ticks);
            LocalDateTime d1 = new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            LocalDateTime d2 = new LocalDateTime(user.CreationDate.Year, user.CreationDate.Month, user.CreationDate.Day, user.CreationDate.Hour, user.CreationDate.Minute, user.CreationDate.Second);
            long days = Period.Between(d2, d1).Days;

            return days == 0 ? totalDreams : totalDreams / days;
        }

        public static string getWaitingTimeMessage(double x, double y)
        {
            if (x != null && x != 0)
            {
                double duration = (y / x) * 24 * 60 * 60;

                LocalDateTime d1 = new LocalDateTime();
                LocalDateTime d2 = d1.PlusSeconds((long)duration);
                Period period = Period.Between(d1, d2);
                int years = period.Years;
                int months = period.Months;
                int days = period.Days;
                long hours = period.Hours;
                long minutes = period.Minutes;
                long seconds = period.Seconds;
                string result = "Your average waiting time is " + (years > 0 ? years + " years " : "") +
                                (months > 0 ? months + " months " : "") +
                                (days > 0 ? days + " days " : "");
                //(hours > 0 ? hours + " hours " : "") +
                // (minutes > 0 ? minutes + " minutes " : "");
                return result;
            }
            return "";
        }

        public double getServiceProviderSpeed(ApplicationUser user, int totalDoneServices)
        {
            //TimeSpan difference = DateTime.Now - user.CreationDate;
            TimeSpan span1 = new TimeSpan(DateTime.Now.Ticks);
            //TimeSpan span2 = new TimeSpan(user.CreationDate.Ticks);
            LocalDateTime d1 = new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            LocalDateTime d2 = new LocalDateTime(user.CreationDate.Year, user.CreationDate.Month, user.CreationDate.Day, user.CreationDate.Hour, user.CreationDate.Minute, user.CreationDate.Second);
            long days = Period.Between(d2, d1).Days;

            return days == 0 ? 0 : totalDoneServices / days;
        }

        public List<Service> getUserServices(String status,string id)
        {
            return db.Services.Where(a => a.Status.Equals(status) && a.ServiceProviderId.Equals(id)).ToList();
        }

        public List<Service> getUserServices(string id)
        {
            return db.Services.Where(a=> a.ServiceProviderId.Equals(id)).ToList();
        }

        public List<Service> getServicesFiltered(List<Service> services,string status) {

            return services.Where(a => a.Status.Equals(status)).ToList();
        }

        public List<ApplicationUser>  getServiceProviders(String code,string status)
        {
            List<UserWorkBinding> bindings = db.UserWorkBindings.Where(a => a.UserWork.Code.Equals(code)
            && a.User.Status.Equals(CoreController.UserStatus.Active.ToString())
            ).Include("User").ToList();
            return bindings.Select(a=>a.User).ToList();
        }


        public UserBalance getUserBalance (ApplicationUser user)
        {
            UserBalance balance = new UserBalance();
            //List<Service> services = db.Services.Where(a => a.ServiceProviderId.Equals(user.Id)&&a.Status.Equals("Active"))
            //    .Include("ServiceProvider")
            //    .Include("ServicePath")
            //    .ToList();
            List<Transaction> transactions = db.Transactions.Where(a => a.UserId.Equals(user.Id)).ToList();
            List<Payment> payments = db.Payments.Where(a => a.Service.ServiceProviderId.Equals(user.Id))
                .Include("Service")
                .Include("Service.ServiceProvider")
                .Include("Service.ServicePath")
                .ToList();
            double doneBalance = 0.0;
            double ActiveBalance = 0.0;
            //foreach (var item in services)
            //{
            //   ActiveBalance += item.ServicePath.Cost * item.ServicePath.Ratio;
            //}
            balance.TransferedBalance =transactions.Count>0? transactions.Sum(a => a.Amount):0.0;
            foreach (var item in payments)
            {
                if(item.Service.Status.Equals("Done"))
                    doneBalance += item.Service.ServicePath.Cost * item.Service.ServicePath.Ratio;
                else
                    ActiveBalance += item.Service.ServicePath.Cost * item.Service.ServicePath.Ratio;
            }
            balance.DoneBalance = doneBalance - balance.TransferedBalance;
            balance.SuspendedBalance = ActiveBalance>=0?ActiveBalance:0.0;

            return balance;

            

        }
        public ApplicationUser findUser(string id)
        {
            return db.Users.Find(id);
        }

    }
}