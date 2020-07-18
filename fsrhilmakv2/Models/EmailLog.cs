using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class EmailLog:BasicModel
    {

        [Required]
        [Display(Name = "Sender")]
        public string Sender { get; set; }

        [Required]
        [Display(Name = "Receiver")]
        public string Receiver { get; set; }


        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }


        [Display(Name = "Body")]
        public string Body { get; set; }


    }
}