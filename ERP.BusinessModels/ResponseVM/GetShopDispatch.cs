using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetShopDispatch
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public GetCreatedBy CreatedBy { get; set; }
        public string Code { get; set; }

        public long StatusId { get; set; }
        public GetStatus Status { get; set; }

        public long ShopOrderId { get; set; }
        public GetShopOrder ShopOrder { get; set; }

        public string Remarks { get; set; }
        public string VehicleNo { get; set; }

        public List<GetShopDispatchDetail> ShopDispatchDetail { get; set; }
    }

    public class GetShopDispatchDetail
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long ShopDispatchId { get; set; }
        public GetShopDispatch ShopDispatch { get; set; }

        public long ShopOrderItemId { get; set; }
        public GetShopOrderItems ShopOrderItem { get; set; }

        public long Quantity { get; set; }
    }


}
