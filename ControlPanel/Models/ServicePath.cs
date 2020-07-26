using ControlPanel.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControlPanel.Models
{
    public class ServicePath:BasicModel
    {
        [Required]
        [Display(Name = "Name")]
        public String Name { get; set; }

        [Required]
        [Display(Name = "Cost")]
        public double Cost { get; set; }

        [Required]
        [Display(Name = "Enabled")]
        public bool Enabled { get; set; }

        [Required]
        [Display(Name = "Is Private")]
        public bool IsPrivate { get; set; }

        public String ServiceProviderId { get; set; }

        public ApplicationUser ServiceProvider { get; set; }

        public String Message { get; set; }

        public double Ratio { get; set; }



    }
}