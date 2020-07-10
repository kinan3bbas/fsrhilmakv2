using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class Service:BasicModel
    {
        [Display(Name = "Status")]
        public String Status { get; set; } //Active, Done,Deleted


        [Display(Name = "Description")]
        public string Description { get; set; }


        public string ServiceProviderId { get; set; }


        public ApplicationUser ServiceProvider { get; set; }


        public int ServicePathId { get; set; }


        public ServicePath ServicePath { get; set; }

        [Display(Name = "Explanation")]
        public string Explanation { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Explanation Date")]
        public DateTime? ExplanationDate { get; set; }

        public long numberOfViews { get; set; }

        public long numberOfLikes { get; set; }

        public String Name { get; set; }

        public String Sex { get; set; }

        public String KidsStatus { get; set; }

        public String SocialStatus { get; set; }

        public bool IsThereWakefulness { get; set; }

        public DateTime? DreamDate { get; set; }

        public String Country { get; set; }

        public bool HaveYouPrayedBeforeTheDream { get; set; }

        public String RegligionStatus { get; set; }

        public bool DidYouExorcism { get; set; }

        public bool PublicServiceAction { get; set; }

        public bool PrivateService { get; set; }

        public double PrivateServicePrice { get; set; }

        public int UserWorkId { get; set; }
        public UserWork UserWork { get; set; }


    }
}