using System;

namespace ERP.Entities.Models
{
    public class SalesTarget : BaseEntity
    {
        public long Target { get; set; }
        public DateTime TargetMonth { get; set; }

        public Guid? UserId { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}