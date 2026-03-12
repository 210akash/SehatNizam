using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetItemGroupDetails
    {
        public long ItemId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public decimal VolumeInMl { get; set; }
        public decimal QuantityInPack { get; set; }
        public string ImageName { get; set; }
        public long? PriceGroupDetailsId { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? TradePrice { get; set; }
        public decimal? DistributorPrice { get; set; }
        public decimal? DistributorPromo { get; set; }
        public decimal? NetDistributorPrice { get; set; }
    }
}
