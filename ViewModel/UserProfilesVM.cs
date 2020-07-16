﻿using Falcon_Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon_Bug_Tracker.ViewModel
{
    public class UserProfilesVM
    {
        public string Id { get; set; } //current user's Id
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public ICollection<Project> ProjectIn { get; set; } //Projects I am assigned to
        public ICollection<Project> ProjectOut { get; set; } //Projects I am not assigned to

        public ICollection<Ticket> TicketsIn { get; set; } //Projects I am involved in
        public string AvatarPath { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Current Password")]
        //public string OldPassword { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name= "New password")]
        //public string NewPassword { get; set; }

        //[DataType(DataType, Password]
        //[Display]


    }
}