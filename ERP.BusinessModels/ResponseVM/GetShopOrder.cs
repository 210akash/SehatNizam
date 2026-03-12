using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetShopOrder
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public GetCreatedBy CreatedBy { get; set; }
        public long? DealershipId { get; set; }
        public long? ShopId { get; set; }
        public GetShop Shop { get; set; }
        public string PaymentMode { get; set; }
        public decimal? Amount { get; set; }
        public long ShopOrderStatusId { get; set; }
        public GetStatus ShopOrderStatus { get; set; }
        public string Remarks { get; set; }
        public List<GetShopOrderItems> ShopOrderItems { get; set; }
        public List<GetShopDispatchDetail> ShopDispatchDetail { get; set; }
    }

    public class GetShopOrderItems
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long ShopOrderId { get; set; }

        public int Quantity { get; set; }
        public long DispatchQuantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }

        public long ItemId { get; set; }
        public GetItem Item { get; set; }

        //public List<GetShopDispatchDetail> ShopDispatchDetails { get; set; }
    }


}
