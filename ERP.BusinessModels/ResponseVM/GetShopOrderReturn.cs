using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetShopOrderReturn
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public long OrderId { get; set; }
        public virtual GetOrder Order { get; set; }

        public long StatusId { get; set; }
        public virtual GetStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
        public GetUser ModifiedBy { get; set; }

        public string Remarks { get; set; }

        public virtual List<GetShopOrderReturnDetail> ShopOrderReturnDetail { get; set; }
    }

    public class GetShopOrderReturnDetail
    {
        public long Id { get; set; }
        public long ShopOrderReturnId { get; set; }
        public virtual GetShopOrderReturn ShopOrderReturn { get; set; }
        public decimal Quantity { get; set; }
        public long OrderItemsId { get; set; }
        public virtual GetOrderItems OrderItems { get; set; }
    }
}
