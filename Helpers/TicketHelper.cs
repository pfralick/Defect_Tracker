using Falcon_Bug_Tracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon_Bug_Tracker.Helpers
{
    public class TicketHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projectHelper = new ProjectsHelper();

        public ICollection<ApplicationUser> AssignableDevelopers(int projectId)
        {
            var developers = roleHelper.UsersInRole("Developer");
            var onProject = projectHelper.UsersOnProject(projectId);
            return developers.Intersect(onProject).ToList();
        }

        public List<Ticket> ListMyTickets()
        {
            var myTickets = new List<Ticket>();
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            switch (myRole)
            {
                case "Admin":
                case "DemoAdmin":
                    myTickets.AddRange(db.Tickets);
                    break;
                case "Project Manager":
                case "DemoPM":
                    //myTickets.AddRange(user.Projects.Where(p => p.IsArchived == false).SelectMany(p => p.Tickets));
                    myTickets.AddRange(db.Projects.Where(p => p.ProjectManagerId == userId).SelectMany(p => p.Tickets));
                    break;
                case "Developer":
                case "DemoDeveloper":
                    myTickets.AddRange(db.Tickets.Where(t => t.IsArchived == false).Where(t => t.DeveloperId == userId));
                    break;
                case "Submitter":
                case "DemoSubmitter":
                    myTickets.AddRange(db.Tickets.Where(t => t.IsArchived == false).Where(t => t.SubmitterId == userId));
                    break;

            }
            return myTickets;
        }
    }
}