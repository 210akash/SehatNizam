using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetOrder
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public GetCreatedBy CreatedBy { get; set; }
        public string DealershipAddress { get; set; }

        public long? DealershipId { get; set; }
        public GetDealership Dealership { get; set; }

        public long? ShopId { get; set; }
        public GetShop Shop { get; set; }

        public Guid? DSFId { get; set; }
        public GetUsers DSF { get; set; }

        public long OrderStatusId { get; set; }
        public GetStatus OrderStatus { get; set; }

        public decimal? BillingAmount { get; set; }
        public decimal? Cash { get; set; }
        public decimal? OnlineTransfer { get; set; }
        public string TransferMode { get; set; }
        public decimal? Credit { get; set; }
        public bool? IsPartial { get; set; }

        public List<GetOrderItems> OrderItems { get; set; }
        public List<GetOrderProcess> OrderProcess { get; set; }
        public List<GetAttachments> OrderAttachments { get; set; }
    }

    public class GetDealershipOrder
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string OrderStatus { get; set; }
        public List<GetDealershipOrderItems> OrderItems { get; set; }
        public List<GetDealershipOrderProcess> OrderProcess { get; set; }
        public List<GetDealershipAttachments> OrderAttachments { get; set; }
    }


}
