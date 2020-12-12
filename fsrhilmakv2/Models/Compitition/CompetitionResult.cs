using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class CompetitionResult:BasicModel
    {
        public int NumberOfActiveServices { get; set; }

        public int NumberOfDoneServices { get; set; }

        public double Speed { get; set; }

        public double TotalBalance { get; set; }

        public double SuspendedBalance { get; set; }

        public double AvailableBalance { get; set; }

        public double AvgServicesInOneDay
        {
            get; set;
        }
        public double Rating { get; set; }

        public string ServiceProviderId { get; set; }
        public ApplicationUser ServiceProvider { get; set; }

        public Competition competition { get; set; }

        public int competitionId { get; set; }

        public int rank { get; set; }

        public long pointsBalance { get; set; }
    }
}