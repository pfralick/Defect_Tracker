using Falcon_Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon_Bug_Tracker.ViewModel
{
    public class ProjectViewModel
    {
        public int ProjectCount { get; set; }
        public List<Project> AllProjects { get; set; }
        public List<ApplicationUser> AllPMs { get; set; }

        public ProjectViewModel()
        {
            AllProjects = new List<Project>();
            AllPMs = new List<ApplicationUser>();
        }
    }
}