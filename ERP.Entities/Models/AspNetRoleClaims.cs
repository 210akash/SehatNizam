using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public partial class AspNetRoleClaims
    {
        public long Id { get; set; }
        public Guid RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public virtual AspNetRoles Role { get; set; }
    }
}
