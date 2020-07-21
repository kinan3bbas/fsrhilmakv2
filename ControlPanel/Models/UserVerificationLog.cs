using ControlPanel.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControlPanel.Models
{
    public class UserVerificationLog:BasicModel
    {



        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "confirmation Date")]
        public DateTime? ConfirmationDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }

      

        [Display(Name = "code")]
        public string Code { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [EmailAddress]
        [Display(Name = "Email ")]
        public String Email { get; set; }

        [Display(Name = "Is Email Sent")]
        public Boolean IsEmailSent { get; set; }


        public String UserId { get; set; } 

        

      
    }
}