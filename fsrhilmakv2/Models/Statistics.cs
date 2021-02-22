using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class Statistics:BasicModel
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

        public string status { get; set; }

        public int AllDreams { get; set; }
        public int AllLaw { get; set; }
        public int AllMedical { get; set; }
        public int AllIstashara { get; set; }

    }
}