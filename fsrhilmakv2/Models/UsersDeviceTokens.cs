using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class UsersDeviceTokens:BasicModel
    {
        [Display (Name ="Token")]
        public string token { get; set; }

        public String UserId { get; set; }

    }
}