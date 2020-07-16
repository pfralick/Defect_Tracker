using Falcon_Bug_Tracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon_Bug_Tracker.Helpers
{
    public class NotificationHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void ManageNotifications(Ticket oldTicket, Ticket newTicket)
        {
            //Manage a Developer assignment notification
            GenerateTicketAssignmentNotifications(oldTicket, newTicket);

            //Manage some other general change notification
            //GenerateTicketChangeNotification(oldTicket, newTicket);
        }

        private void GenerateTicketAssignmentNotifications(Ticket oldTicket, Ticket newTicket)
        {
            bool assigned = oldTicket.DeveloperId == null && newTicket.DeveloperId != null;
            bool unassigned = oldTicket.DeveloperId != null && newTicket.DeveloperId == null;
            bool reassigned = !assigned && !unassigned && oldTicket.DeveloperId != newTicket.DeveloperId;

            if (assigned)
            {
                var created = DateTime.Now;
                db.TicketNotifications.Add(new TicketNotification
                {
                    Created = created,
                    TicketId = newTicket.Id,
                    SenderId = HttpContext.Current.User.Identity.GetUserId(),
                    RecipientId = newTicket.DeveloperId,
                    Subject = "You have been assigned to a Ticket",
                    Body = $"You have been assigned to Ticket Id: {newTicket.Id} on {created.ToString("MMM dd, yyyy")}. This Ticket is on the {newTicket.Project.Name} project."

                });

            }

            db.SaveChanges();
        }

        private void GenerateTicketChangeNotification(TicketHelper oldTicket, TicketHelper newTicket)
        {


        }
    }    
}