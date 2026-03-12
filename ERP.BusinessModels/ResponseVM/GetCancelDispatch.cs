using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetCancelDispatch
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public long OrderId { get; set; }
        public virtual GetOrder Order { get; set; }

        public long? StatusId { get; set; }
        public virtual GetStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public virtual List<GetCancelDispatchDetail> CancelDispatchDetail { get; set; }
        public virtual List<GetOrderProcess> OrderProcess { get; set; }

    }

    public class GetCancelDispatchDetail
    {
        public long CancelDispatchId { get; set; }
        public virtual GetCancelDispatch CancelDispatch { get; set; }

        public long OrderItemId { get; set; }
        public virtual GetOrderItems OrderItem { get; set; }

        public long Quantity { get; set; }
    }
}
