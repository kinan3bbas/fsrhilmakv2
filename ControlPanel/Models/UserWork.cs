using ControlPanel.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlPanel.Models
{
    public class UserWork:BasicModel
    {
        public String Name { get; set; }

        public String AdjectiveName { get; set; }

        public bool Enabled { get; set; }
    }
}