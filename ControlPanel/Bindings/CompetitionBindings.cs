using ControlPanel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControlPanel.Bindings
{
    public class CompetitionBindings
    {
        public int id { get; set; }
        [Display(Name = "Status")]
        public String Status { get; set; } //Active, Done,Deleted

        [Display(Name = "Goal")]
        public String Goal { get; set; } //Active, Done,Deleted

        [Display(Name = "Name")]
        public String Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        public int UserWorkId { get; set; }
        public UserWork UserWork { get; set; }

        public CompetitionPrize prize { get; set; }

        public int prizeId { get; set; }

        public int duration { get; set; }

        public bool repeat { get; set; }

        public double rank1 { get; set; }
        public double rank2 { get; set; }
        public double rank3 { get; set; }
        public double rank4 { get; set; }
        public double rank5 { get; set; }
        public double rank6 { get; set; }
        public double rank7 { get; set; }
        public double rank8 { get; set; }
        public double rank9 { get; set; }
        public double rank10 { get; set; }
        public double rank11 { get; set; }
        public double rank12 { get; set; }
        public double rank13 { get; set; }
        public double rank14 { get; set; }
        public double rank15 { get; set; }
        public double rank16 { get; set; }
        public double rank17 { get; set; }
        public double rank18 { get; set; }
        public double rank19 { get; set; }
        public double rank20 { get; set; }
        public double rank21 { get; set; }
        public double rank22 { get; set; }
        public double rank23 { get; set; }
        public double rank24 { get; set; }
        public double rank25 { get; set; }
        public double rank26 { get; set; }
        public double rank27 { get; set; }
        public double rank28 { get; set; }
        public double rank29 { get; set; }
        public double rank30 { get; set; }
        public double rank31 { get; set; }
        public double rank32 { get; set; }
        public double rank33 { get; set; }
        public double rank34 { get; set; }
        public double rank35 { get; set; }
        public double rank36 { get; set; }
        public double rank37 { get; set; }
        public double rank38 { get; set; }
        public double rank39 { get; set; }
        public double rank40 { get; set; }
        public double rank41 { get; set; }
        public double rank42 { get; set; }
        public double rank43 { get; set; }
        public double rank44 { get; set; }
        public double rank45 { get; set; }
        public double rank46 { get; set; }
        public double rank47 { get; set; }
        public double rank48 { get; set; }
        public double rank49 { get; set; }
        public double rank50 { get; set; }
        public double rank51 { get; set; }
        public double rank52 { get; set; }
        public double rank53 { get; set; }
        public double rank54 { get; set; }
        public double rank55 { get; set; }
        public double rank56 { get; set; }
        public double rank57 { get; set; }
        public double rank58 { get; set; }
        public double rank59 { get; set; }
        public double rank60 { get; set; }
        public double rank61 { get; set; }
        public double rank62 { get; set; }
        public double rank63 { get; set; }
        public double rank64 { get; set; }
        public double rank65 { get; set; }
        public double rank66 { get; set; }
        public double rank67 { get; set; }
        public double rank68 { get; set; }
        public double rank69 { get; set; }
        public double rank70 { get; set; }
        public double rank71 { get; set; }
        public double rank72 { get; set; }
        public double rank73 { get; set; }
        public double rank74 { get; set; }
        public double rank75 { get; set; }
        public double rank76 { get; set; }
        public double rank77 { get; set; }
        public double rank78 { get; set; }
        public double rank79 { get; set; }
        public double rank80 { get; set; }
        public double rank81 { get; set; }
        public double rank82 { get; set; }
        public double rank83 { get; set; }
        public double rank84 { get; set; }
        public double rank85 { get; set; }
        public double rank86 { get; set; }
        public double rank87 { get; set; }
        public double rank88 { get; set; }
        public double rank89 { get; set; }
        public double rank90 { get; set; }
        public double rank91 { get; set; }
        public double rank92 { get; set; }
        public double rank93 { get; set; }
        public double rank94 { get; set; }
        public double rank95 { get; set; }
        public double rank96 { get; set; }
        public double rank97 { get; set; }
        public double rank98 { get; set; }
        public double rank99 { get; set; }
        public double rank100 { get; set; }



    }
}