using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Bindings
{
    public class PaymentBinding
    {
        public int ServicePathId { get; set; }
       
        public int ServiceId { get; set; }

        public double Amount { get; set; }
        public string Method { get; set; }

        public string Currency { get; set; }

        public bool UseUserPoints { get; set; }

        public bool PrivateService { get; set; }

        public long NumberOfPoints { get; set; }

    }
}