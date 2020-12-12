using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class PublicService
    {

        public String Status { get; set; } //Active, Done,Deleted


        public string Description { get; set; }


        public string ServiceProviderId { get; set; }


        public ApplicationUser ServiceProvider { get; set; }


        public int ServicePathId { get; set; }



        public string Explanation { get; set; }


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

        public String JobStatus { get; set; }

        //public ICollection<ServiceComment> Comments { get; set; }


        public int id { get; set; }


        public ServicePath ServicePath { get; set; }

        public long NumberOfRemainingPeople { get; set; }

        public long NumberOfAllPeopleWaiting { get; set; }

        public String AvgWaitingTime { get; set; }


        //public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Last Modification Date")]
        public DateTime LastModificationDate { get; set; }

        public ApplicationUser Creator { get; set; }

        public String CreatorId { get; set; }

        public ApplicationUser Modifier { get; set; }
        public String ModifierId { get; set; }

        public long AttachmentId { get; set; }

        public String ServiceProviderToken { get; set; }

        public String ClientToken { get; set; }

        public double ServiceProviderSpeed { get; set; }

        public double ServiceProviderAvgServices { get; set; }

        public int ServiceId { get; set; }
    }
}