using Falcon_Bug_Tracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon_Bug_Tracker.Helpers
{
    public class HistoryHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void ManageHistoryRecordCreation(Ticket oldTicket, Ticket newTicket)
        {
            //Now I can compare the new Ticket, to the old Ticket

            if(oldTicket.Title != newTicket.Title)
            {
                var newHistoryRecord = new TicketHistory();

                newHistoryRecord.Created = (DateTime)newTicket.Updated;
                newHistoryRecord.UserId = HttpContext.Current.User.Identity.GetUserId();
                newHistoryRecord.Property = "Title";
                newHistoryRecord.OldValue = oldTicket.Title;
                newHistoryRecord.NewValue = newTicket.Title;
                newHistoryRecord.TicketId = newTicket.Id;

                db.TicketHistories.Add(newHistoryRecord);
                db.SaveChanges();                              
            }

            if (oldTicket.DeveloperId != newTicket.DeveloperId)
            {
                    
                var newHistoryRecord = new TicketHistory();

                newHistoryRecord.Created = (DateTime)newTicket.Updated;
                newHistoryRecord.UserId = HttpContext.Current.User.Identity.GetUserId();
                newHistoryRecord.Property = "DeveloperId";
                newHistoryRecord.OldValue = oldTicket.DeveloperId;
                newHistoryRecord.NewValue = newTicket.DeveloperId;
                newHistoryRecord.TicketId = newTicket.Id;

                db.TicketHistories.Add(newHistoryRecord);
                db.SaveChanges();

            }
                    
                    
                    
                    
                    
                    }



    }
}