﻿using ControlPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlPanel.ViewModels
{
    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public string PersonalDescription { get; set; }
        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }

        public string Sex { get; set; }

        public string Country { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string MartialStatus { get; set; }

        public string JobDescription { get; set; }

        public DateTime? JoiningDate { get; set; }

        public string PictureId { get; set; }

        public int AvgServicesInOneDay { get; set; }

        public int Age { get; set; }

        public string phoneNumber { get; set; }

        public string FireBaseId { get; set; }

        public string Id { get; set; }

        public int NumberOfActiveServices { get; set; }

        public int NumberOfDoneServices { get; set; }

        public string SocialStatus { get; set; }

        public string UserName { get; set; }

        public Boolean VerifiedUser { get; set; }

        public List<UserWorkBinding> UserWorks { get; set; }
        public List<string> UserRoles { get; set; }

        public double Speed { get; set; }
    }
}