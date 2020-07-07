
using fsrhilmakv2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Extras
{
    public class BasicModel
    {
        [Key]
        public int id { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Last Modification Date")]
        public DateTime LastModificationDate { get; set; }

        public ApplicationUser Creator { get; set; }

        public String CreatorId { get; set; }

        public ApplicationUser Modifier { get; set; }
        public String ModifierId { get; set; }

        public long AttachmentId { get; set; }


    }
}