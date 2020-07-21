using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.ViewModels
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


    }
}