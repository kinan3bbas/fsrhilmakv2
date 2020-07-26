using ControlPanel.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControlPanel.Models
{
    public class Payment:BasicModel
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; }



        public double Amount { get; set; }
        public string Method { get; set; }

        public string Currency { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }




    }
}