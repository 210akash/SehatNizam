using System;

namespace ERP.Entities.Models
{
    public class UserProject : BaseEntity
    {
        public long ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public Guid UserId { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
