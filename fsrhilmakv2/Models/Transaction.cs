using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class Transaction:BasicModel
    {
        public double Amount { get; set; }
        public String Bank { get; set; }

        public String method { get; set; }

        public String Status { get; set; }
        public String UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}