using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class RetailOrderReturn : BaseEntity
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long RetailOrderId { get; set; }
        public virtual RetailOrder RetailOrder { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }

        public virtual List<RetailOrderReturnDetail> RetailOrderReturnDetail { get; set; }
    }
}
