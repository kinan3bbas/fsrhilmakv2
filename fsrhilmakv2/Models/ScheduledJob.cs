using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class ScheduledJob:BasicModel
    {

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }


        [Required]
        [Display(Name = "interval")]
        public int interval { get; set; }


        [Display(Name = "Status")]
        public string Status { get; set; }


    }
}