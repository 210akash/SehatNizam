using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public partial class AspNetUserClaims
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
