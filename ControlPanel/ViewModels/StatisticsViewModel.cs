using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlPanel.ViewModels
{
    public class StatisticsViewModel
    {
        public int AllServices { get; set; }
        public int AllDoneServices { get; set; }
        public int AllActiveServices{ get; set; }

        public int AllServiceProviders { get; set; }
        public int AllInterpreters { get; set; }
        public int AllIftaa { get; set; }
        public int AllRouqat { get; set; }
        public int AllMustashareen { get; set; }
        public int AllClients { get; set; }

        public int AllActiveClients { get; set; }

        public int AllActiveClientsInThePastThreeDays { get; set; }

        public int AllUsers { get; set; }


        public int AllDreamUsers { get; set; }

        public int AllRouqiaUsers { get; set; }
        public int AllIstasharaUsers { get; set; }
        public int AllMedicalUsers { get; set; }
        public int AllIftaaUsers { get; set; }
        public int AllLawUsers { get; set; }

        public double TotalBalance { get; set; }
        public double AvailableBalance { get; set; }
        public double SuspendedBalance { get; set; }

        public double AllPaymentsSum { get; set; }
        public double Profit { get; set; }

    }
}