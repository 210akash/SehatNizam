using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetRetailOrderReturn
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public long RetailOrderId { get; set; }
        public virtual GetRetailOrder RetailOrder { get; set; }

        public long StatusId { get; set; }
        public virtual GetStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
        public GetUser ModifiedBy { get; set; }

        public string Remarks { get; set; }

        public virtual List<GetRetailOrderReturnDetail> RetailOrderReturnDetail { get; set; }
    }

    public class GetRetailOrderReturnDetail
    {
        public long Id { get; set; }
        public long RetailOrderReturnId { get; set; }
        public virtual GetRetailOrderReturn RetailOrderReturn { get; set; }
        public decimal Quantity { get; set; }
        public long RetailOrderItemsId { get; set; }
        public virtual GetRetailOrderItems RetailOrderItems { get; set; }
    }
}
