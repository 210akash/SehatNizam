using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetRetailOrder
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }
        public GetCreatedBy CreatedBy { get; set; }

        public long? ShopId { get; set; }
        public GetShop Shop { get; set; }

        public long RetailOrderStatusId { get; set; }
        public GetStatus RetailOrderStatus { get; set; }

        public string Comments { get; set; }
        public string Reference { get; set; }
        public string Department { get; set; }

        public List<GetRetailOrderItems> RetailOrderItems { get; set; }
        public List<GetRetailOrderProcess> RetailOrderProcess { get; set; }
    }

    public class GetRetailOrderItems
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long RetailOrderId { get; set; }
        public GetRetailOrder RetailOrder { get; set; }

        public int Quantity { get; set; }
        public int? ShippedQuantity { get; set; }
        public decimal? DistributorPromo { get; set; }
        public decimal DistributorPrice { get; set; }
        public decimal? CustomDistributorPrice { get; set; }
        public decimal TradePrice { get; set; }
        public decimal? CustomTradePrice { get; set; }
        public decimal? RetailPrice { get; set; }

        public long ItemId { get; set; }
        public GetItem Item { get; set; }

        public int? HoldQuantity { get; set; }
        public int? LeftQuantity { get; set; }
    }


}