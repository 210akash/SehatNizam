using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Entities.Models
{
    public class UserAppAccess
    {
        public long Id { get; set; }
        public Guid RoleId { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
