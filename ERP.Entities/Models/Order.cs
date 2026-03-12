using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Order : BaseEntity
    {
        public string DealershipAddress { get; set; }
        public decimal? BillingAmount { get; set; }
        public decimal? Cash { get; set; }
        public decimal? OnlineTransfer { get; set; }
        public string TransferMode { get; set; }
        public decimal? Credit { get; set; }
        public bool? IsPartial { get; set; }

        public long? DealershipId { get; set; }
        public virtual Dealership Dealership { get; set; }

        public long? ShopId { get; set; }
        public virtual Shop Shop { get; set; }

        public long OrderStatusId { get; set; } 
        public virtual Status OrderStatus { get; set; }

        public Guid? DSFId { get; set; }
        public virtual AspNetUsers DSF { get; set; }

        public virtual ICollection<OrderItems> OrderItems { get; set; } 
        public virtual ICollection<OrderProcess> OrderProcess { get; set; }
        public virtual ICollection<Attachments> OrderAttachments { get; set; }
        public virtual ICollection<DispatchOrder> DispatchOrder { get; set; }
        public virtual ICollection<CancelDispatch> CancelDispatch { get; set; }
    }
}
