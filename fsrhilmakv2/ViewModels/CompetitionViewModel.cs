using fsrhilmakv2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.ViewModels
{
    public class CompetitionViewModel
    {
        [Display(Name = "Status")]
        public String Status { get; set; } //Active, Done,Deleted

        [Display(Name = "Goal")]
        public String Goal { get; set; } //Active, Done,Deleted

        [Display(Name = "Name")]
        public String Name { get; set; }


        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }


        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        public UserWork UserWork { get; set; }

        public double FirstPlacePrice { get; set; }


        

        public int duration { get; set; }

        public bool repeat { get; set; }
        
        public int id { get; set; }
    }
}