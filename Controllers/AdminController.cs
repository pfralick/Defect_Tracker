using Falcon_Bug_Tracker.Helpers;
using Falcon_Bug_Tracker.Models;
using Falcon_Bug_Tracker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Falcon_Bug_Tracker.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        // GET: Admin
        public ActionResult ManageRoles()
        {
            //Setup some data that can be used inside the view
            var viewData = new List<UserRoleData>();
            var users = db.Users.ToList();
            foreach(var user in users)
            {
                var newUserData = new UserRoleData();
                
                newUserData.FirstName = user.FirstName;
                newUserData.LastName = user.LastName;
                newUserData.Email = user.Email;
                newUserData.RoleName = roleHelper.ListUserRoles(user.Id).FirstOrDefault() ?? "UnAssigned";

                viewData.Add(newUserData);
            }


            //Left hand side control: This data will be used to power ListBox in the View
            ViewBag.UserIds = new MultiSelectList(db.Users, "Id", "Email");

            //Right hand side control: This data will be used to power a Dropdown List in the View
            ViewBag.RoleName = new SelectList(db.Roles, "Name", "Name");
           
            return View(viewData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageRoles(List<string> userIds, string roleName)
        {
            //Use our RoleHelper to actually assign the role to the person or persons selected
            if(userIds != null)
            {
                //First I have to remove each of the selected users from their current Role.
                foreach(var userId in userIds)
                {
                    //First I need to determine what Role if any the user is in
                    var userRole = roleHelper.ListUserRoles(userId).FirstOrDefault();

                    if (userRole != null)
                    {

                        //First I have to remove each of the selected users from their current Role.
                        roleHelper.RemoveUserFromRole(userId, userRole);
                    }

                    //Then I will add each selected user to the selected Role.
                    roleHelper.AddUserToRole(userId, roleName);
                }


            }
            return RedirectToAction("ManageRoles");
        }


    }
}



    //public class AdminController : Controller
    //{
    //    private UserRolesHelper rolesHelper = new UserRolesHelper();
    //    // GET: Admin
    //    public ActionResult AskAboutRoles()
    //    {
    //        if (rolesHelper.UsersInRole("Developer").Count == 0)
    //        {
    //            return View();
    //        }
    //        return View();
    //    }

    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult ManageRoles(List<string> userIds, string roleName)
    //    {


    //    }
    //}