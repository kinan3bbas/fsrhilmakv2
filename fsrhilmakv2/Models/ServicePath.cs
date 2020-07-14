using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class ServicePath:BasicModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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


    }
}