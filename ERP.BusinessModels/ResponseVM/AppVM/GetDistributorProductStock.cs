using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BusinessModels.ResponseVM.AppVM
{
    public class GetDistributorProductStock
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long Quantity { get; set; }
        public String Type { get; set; }
        public decimal VolumeInMl { get; set; }
        public decimal QuantityInPack { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal TradePrice { get; set; }
        public decimal DistributorPrice { get; set; }
        public int LeftQuantity { get; set; }
        public int HoldQuantity { get; set; }
        public int TransitQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string ProductImagePath { get; set; }
    }
}
