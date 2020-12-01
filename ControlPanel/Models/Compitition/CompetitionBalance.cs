using ControlPanel.Extras;
using ControlPanel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControlPanel.Models
{
    public class CompetitionBalance : BasicModel
    {
        public String ServiceProviderId { get; set; }
        public ApplicationUser ServiceProvider { get; set; }
        public double Amount { get; set; }
        public string Method { get; set; }

        public string Currency { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }


    }
}