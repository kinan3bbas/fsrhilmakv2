﻿using System;
using System.Collections.Generic;

namespace fsrhilmakv2.Models
{
    // Models returned by AccountController actions.

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

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

        public int numbOfDreamsInOneDay { get; set; }

        public int Age { get; set; }

        public string phoneNumber { get; set; }

        public string FireBaseId { get; set; }

        public string Id { get; set; }

        public int NumberOfActiveDreams { get; set; }

        public int NumberOfDoneDreams { get; set; }

        public string SocialStatus { get; set; }

        public string UserName { get; set; }

        public Boolean VerifiedInterpreter { get; set; }

        public List<UserWorkBinding> UserWorks { get; set; }
        public List<string> UserRoles { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}