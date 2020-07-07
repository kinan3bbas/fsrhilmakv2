using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class UserWork:BasicModel
    {
        public String Name { get; set; }

        public String AdjectiveName { get; set; }

        public bool Enabled { get; set; }
    }
}