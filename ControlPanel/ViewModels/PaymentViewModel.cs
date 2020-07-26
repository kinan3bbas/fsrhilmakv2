using ControlPanel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControlPanel.ViewModels
{
    public class PaymentViewModel
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public double Amount { get; set; }
        public string Method { get; set; }

        public string Currency { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public int id { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Last Modification Date")]
        public DateTime LastModificationDate { get; set; }

        public ApplicationUser Creator { get; set; }

        public String CreatorId { get; set; }
    }
}