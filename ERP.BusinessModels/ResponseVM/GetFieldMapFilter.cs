using ERP.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetFieldMapFilterSP
    {
        public List<GetRegionLite> RegionList { get; set; }
        public List<GetZoneLite> ZoneList { get; set; }
        public List<GetAreaLite> AreaList { get; set; }
        public List<GetTerritoryLite> TerritoryList { get; set; }
        public List<GetDealershipLite> DealershipList { get; set; }
        public List<GetShopLite> ShopList { get; set; }
    }

    public class GetFieldMapFilterEF
    {
        public List<GetRegion> RegionList { get; set; }
        public List<GetZone> ZoneList { get; set; }
        public List<GetArea> AreaList { get; set; }
        public List<GetTerritory> TerritoryList { get; set; }
        public List<GetDealership> DealershipList { get; set; }
        public List<GetShop> ShopList { get; set; }
    }
}
