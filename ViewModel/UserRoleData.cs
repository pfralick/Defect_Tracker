using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Falcon_Bug_Tracker.ViewModel
{
    public class UserRoleData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }

        public string FullName
        { 
            get
            {
                return $"{LastName}, {FirstName}";
            }
        }


    }
}