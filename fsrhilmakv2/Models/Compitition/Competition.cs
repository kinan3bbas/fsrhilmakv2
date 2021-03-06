﻿using fsrhilmakv2.Extras;
using fsrhilmakv2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class Competition : BasicModel
    {
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
        public DateTime EndDate { get; set; }

        public int UserWorkId { get; set; }
        public UserWork UserWork { get; set; }

        public CompetitionPrize prize { get; set; }

        public int prizeId { get; set; }

        public int duration { get; set; }

        public bool repeat { get; set; }


        public int? ParentCompetitionId { get; set; }
        public Competition ParentCompetition { get; set; }

        public long pointsBalance { get; set; }

    }
}