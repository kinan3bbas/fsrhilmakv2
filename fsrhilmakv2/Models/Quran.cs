using fsrhilmakv2.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Models
{
    public class Quran : BasicModel
    {

  

        [Required]
        [Display(Name = "Text")]
        public string Text { get; set; }


        [Required]
        [Display(Name = "Number")]
        public int Number { get; set; }


        [Display(Name = "Surat")]
        public string Surat { get; set; }




    }
}