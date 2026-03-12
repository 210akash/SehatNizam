using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class DispatchOrder : BaseEntity
    {
        public long OrderId { get; set; }
        public virtual Order Order { get; set; }

        public string DCCode { get; set; }
        public string INVCode { get; set; }

        public decimal OrderFreightCharges { get; set; }
        public decimal DistributorAmount { get; set; }
        public decimal TradePromo { get; set; }
        public decimal TradeMargin { get; set; }
        public decimal DistributorMargin { get; set; }

        public long DispatchId { get; set; }
        public virtual Dispatch Dispatch { get; set; }

        public long? StatusId { get; set; }
        public virtual Status Status { get; set; }

        public DateTime? ReceivedDate { get; set; }
        public Guid? ReceivedById { get; set; }
        public virtual AspNetUsers ReceivedBy { get; set; }

        public DateTime? PrintDate { get; set; }
        public Guid? PrintById { get; set; }
        public virtual AspNetUsers PrintBy { get; set; }

        public virtual List<DispatchDetail> DispatchDetail { get; set; }
    }

    public class DispatchDetail : BaseEntity
    {
        public long OrderItemId { get; set; }
        public virtual OrderItems OrderItem { get; set; }

        public long? CostSheetId { get; set; }
        public virtual CostSheet CostSheet { get; set; }

        public long Quantity { get; set; }

        public long DispatchOrderId { get; set; }
        public virtual DispatchOrder DispatchOrder { get; set; }

    }
}
