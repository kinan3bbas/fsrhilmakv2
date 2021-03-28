using fsrhilmakv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.ViewModels
{
    public class StatisticsMainPage
    {
        public Statistics statistics { get; set; }

        public List<Service> services { get; set; }
    }
}