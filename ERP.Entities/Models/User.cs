using System;
using System.Collections.Generic;

namespace CRM.Entities.Models
{
    public partial class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleID { get; set; }
        public bool PhoneNotifications { get; set; }
        public bool EmailNotifications { get; set; }
        public virtual Role Role { get; set; }
    }
}
