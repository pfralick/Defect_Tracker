using Falcon_Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon_Bug_Tracker.Helpers
{
    public class HistoriesDisplayHelper
    {
        public static string DisplayData(TicketHistory ticketHistory)
        {
            var db = new ApplicationDbContext();

            var data = "";

            switch(ticketHistory.Property)
            {
                case "DeveloperId":
                    var tempData = db.Users.FirstOrDefault(u => u.Id == ticketHistory.NewValue);
                    data = $"{tempData.LastName}, {tempData.FirstName}";
                    break;
                default:
                    data = ticketHistory.NewValue;
                    break;
            }
            return data;
        }
    }
}