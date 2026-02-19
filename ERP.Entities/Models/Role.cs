using System;
using System.Collections.Generic;

namespace CRM.Entities.Models
{
    public partial class Role
    {
        public Role()
        {
            User = new HashSet<User>();
        }

        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public int? LastModifiedBy { get; set; }
        public Guid? CreatedBy { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
