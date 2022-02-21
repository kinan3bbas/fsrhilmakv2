using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class UserInfoCash
    {

        [Key]
        public int idd
        {
            get; set;
        }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Last Modification Date")]
        public DateTime LastModificationDate { get; set; }
        public string Email { get; set; }

        public string PersonalDescription { get; set; }
        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }

        public string Sex { get; set; }

        public string Country { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string MartialStatus { get; set; }

        public string JobDescription { get; set; }

        public DateTime? JoiningDate { get; set; }

        public string PictureId { get; set; }
        public string PictureFileName { get; set; }

        public double AvgServicesInOneDay { get; set; }

        public int Age { get; set; }

        public string phoneNumber { get; set; }

        public string FireBaseId { get; set; }

        public string Id { get; set; }

        public int NumberOfActiveServices { get; set; }

        public int NumberOfDoneServices { get; set; }

        public string SocialStatus { get; set; }

        public string UserName { get; set; }

        public Boolean VerifiedUser { get; set; }


        public double Speed { get; set; }

        public String UserSpecialCode { get; set; }

        public String UserRegistrationCode { get; set; }

        public long PointsBalance { get; set; }

        public String SocialToken { get; set; }

        public String ImageUrl { get; set; }

        public double TotalBalance { get; set; }

        public double SuspendedBalance { get; set; }

        public double AvailableBalance { get; set; }

        public long ServiceProviderPoints { get; set; }

        public long numberOfFreeServices { get; set; }

        public int rank { get; set; }
    }


}
