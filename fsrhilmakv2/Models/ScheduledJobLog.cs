using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class ScheduledJobLog: BasicModel
    {

        public int scheduledJobId { get; set; }
        public ScheduledJob scheduledJob { get; set; }

        public string error { get; set; }


    }
}