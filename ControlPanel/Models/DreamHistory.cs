using ControlPanel.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlPanel.Models
{
    public class DreamHistory : BasicModel
    {
        public String OldInterpreterId { get; set; }
        public ApplicationUser OldInterpreter { get; set; }

        public String NewInterpreterId { get; set; }
        public ApplicationUser NewInterpreter { get; set; }

        public int ServiceId { get; set; }

        public Service Service {get;set;}
    }
}