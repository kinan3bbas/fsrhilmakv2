using fsrhilmakv2.Controllers;
using fsrhilmakv2.Models;
using NodaTime;
using System;
using System.Collections.Generic;
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

            return days == 0 ? 0 : totalDreams / days;
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

        public ApplicationUser findUser(string id)
        {
            return db.Users.Find(id);
        }

    }
}