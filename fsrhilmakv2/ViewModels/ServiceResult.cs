using fsrhilmakv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.ViewModels
{
    public class ServiceResult
    {
        public Service service { get; set; }

        public List<Service> services { get; set; }
    }
}