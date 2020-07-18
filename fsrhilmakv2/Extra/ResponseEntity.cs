using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Extra
{
    public class ResponseEntity
    {
        public string status { get; set; }
        public string message { get; set; }

        public void addError(String message)
        {
            this.status = "500";
            this.message = message;
        }
        public void addSuccess(String message)
        {
            this.status = "200";
            this.message = message;
        }
    }
}