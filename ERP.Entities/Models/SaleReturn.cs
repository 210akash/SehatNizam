using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class SaleReturn : BaseEntityHistory
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long DispatchOrderId { get; set; }
        public virtual DispatchOrder DispatchOrder { get; set; }

        public long? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }

        public virtual List<SaleReturnDetail> SaleReturnDetail { get; set; }
    }
}
