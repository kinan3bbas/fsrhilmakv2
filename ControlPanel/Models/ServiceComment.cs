using ControlPanel.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControlPanel.Models
{
    public class ServiceComment:BasicModel
    {
        [Display(Name = "Text")]
        public String Text { get; set; }

        public int ServiceId { get; set; }

        public Service Service { get; set; }


    }
}