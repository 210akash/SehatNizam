using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public partial class AspNetUserTokens
    {
        public Guid UserId { get; set; }
        public string LoginProvider { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string Value { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
