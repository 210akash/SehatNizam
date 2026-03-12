using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetSaleReturn
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public long DispatchOrderId { get; set; }
        public virtual GetDispatchOrder DispatchOrder { get; set; }

        public long ProjectId { get; set; }
        public GetProject Project { get; set; }

        public long StatusId { get; set; }
        public virtual GetStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
        public GetUser ModifiedBy { get; set; }

        public GetUser ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetUser ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }


        public string Remarks { get; set; }

        public virtual List<GetSaleReturnDetail> SaleReturnDetail { get; set; }
    }

    public class GetSaleReturnDetail
    {
        public long Id { get; set; }
        public long SaleReturnId { get; set; }
        public virtual GetSaleReturn SaleReturn { get; set; }
        public decimal Quantity { get; set; }

        public long DispatchDetailId { get; set; }
        public virtual GetDispatchDetail DispatchDetail { get; set; }
    }
}
