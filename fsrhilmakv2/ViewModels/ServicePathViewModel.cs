using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.ViewModels
{
    public class ServicePathViewModel
    {
        public String Name { get; set; }

        public double Cost { get; set; }

        public long NumberOfPeopleWaiting { get; set; }

        public String AvgWaitingTime { get; set; }
    }
}