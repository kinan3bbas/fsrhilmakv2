using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class UserWorkBinding:BasicModel
    {
     public int UserWorkId { get; set; }
     public UserWork UserWork { get; set; }

    public String UserId { get; set; }
    public ApplicationUser User { get; set; }
    }
}