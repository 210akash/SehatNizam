using System;

namespace ERP.Entities.Models
{
    public class GST : BaseEntity
    {
        public DateTime FDate { get; set; }
        public DateTime TDate { get; set; }
        public decimal GSTPer { get; set; }
        public long? CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
