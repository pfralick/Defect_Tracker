﻿using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Falcon_Bug_Tracker.Models;
using System.Web;

//namespace BugSleuth2.Models

namespace Falcon_Bug_Tracker.Helpers
{
    public class UserRolesHelper
    {
        private ApplicationDbContext Db = new ApplicationDbContext();
        private string UserId { get; set; }

        public UserRolesHelper()
        {
            UserId = HttpContext.Current.User.Identity.GetUserId();
        }

        public string GetUserDisplayName()
        {
            return Db.Users.Find(UserId).DisplayName;
        }

        public string GetUserAvatar()
        {
            return Db.Users.Find(UserId).AvatarPath;

        }


        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(
                                                            new UserStore<ApplicationUser>(
                                                                new ApplicationDbContext()));

        private ApplicationDbContext db = new ApplicationDbContext();
        public bool IsUserInRole(string userId, string roleName)
        {
            return userManager.IsInRole(userId, roleName);
        }
        public ICollection<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }
        public bool AddUserToRole(string userId, string roleName)
        {
            var result = userManager.AddToRole(userId, roleName);
            return result.Succeeded;
        }
        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = userManager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }
        public ICollection<ApplicationUser> UsersInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            var List = userManager.Users.ToList();
            foreach (var user in List)
            {
                if (IsUserInRole(user.Id, roleName))
                    resultList.Add(user);
            }
            return resultList;
        }

        public ICollection<ApplicationUser> UsersNotInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            var List = userManager.Users.ToList();
            foreach (var user in List)
            {
                if (!IsUserInRole(user.Id, roleName))
                    resultList.Add(user);
            }
            return resultList;
        }






    }
}