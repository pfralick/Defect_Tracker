using Falcon_Bug_Tracker.Models;
using Falcon_Bug_Tracker.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Falcon_Bug_Tracker.Controllers
{
    [Authorize]
    public class UserProfilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult EditProfile()
        {
            //Store the Primary Key of the User in the currentUserId variable
            var currentUserId = User.Identity.GetUserId();

            //The current User variable represents the enitre User record
            var currentUser = db.Users.Find(currentUserId);

            var userProfileVM = new UserVM();
            userProfileVM.Id = currentUser.Id;
            userProfileVM.FirstName = currentUser.FirstName;
            userProfileVM.LastName = currentUser.LastName;
            userProfileVM.DisplayName = currentUser.DisplayName;
            userProfileVM.Email = currentUser.Email;
            
            return View(userProfileVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(UserVM model)
        {
            //I want to get a tracked User record based on the incoming model.Id
            var currentUser = db.Users.Find(model.Id);
            currentUser.FirstName = model.FirstName;
            currentUser.LastName = model.LastName;
            currentUser.DisplayName = model.DisplayName;
            currentUser.Email = model.Email;
            currentUser.UserName = model.Email;

            db.SaveChanges();

            return RedirectToAction("EditProfile");
  
        }



    }
}