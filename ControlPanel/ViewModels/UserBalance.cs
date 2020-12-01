using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlPanel.ViewModels
{
    public class UserBalance
    {
        public double DoneBalance { get; set; }

        public double TransferedBalance { get; set; }

        public double SuspendedBalance { get; set; }

        public double CompetitionBalance { get; set; }
    }
}