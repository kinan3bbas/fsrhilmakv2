﻿using ControlPanel.Extras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControlPanel.Models
{
    public class UserWork
    {
        [Key]
        public int id { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Last Modification Date")]
        public DateTime LastModificationDate { get; set; }
        public String Name { get; set; }

        public String AdjectiveName { get; set; }

        public bool Enabled { get; set; }

        public String Code { get; set; }
    }
}