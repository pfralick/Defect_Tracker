using Falcon_Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon_Bug_Tracker.ViewModel
{
    public class DashboardViewModel
    {
        public int TicketCount { get; set; }
        public int HighPriorityTicketCount { get; set; }
        public int MediumPriorityTicketCount { get; set; }
        public int LowPriorityTicketCount { get; set; }
        public int NewTicketCount { get; set; }
        public int TotalComments { get; set; }

        public List<Ticket> AllTickets { get; set; }

        public ProjectViewModel ProjectVM { get; set; }

        public DashboardViewModel()
        {
            AllTickets = new List<Ticket>();
            ProjectVM = new ProjectViewModel();
        }
    }
}