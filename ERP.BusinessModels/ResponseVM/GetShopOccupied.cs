using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetShopOccupied
    {
        public bool? IsShopOccupied { get; set; }
        public List<GetRoute> RoutesInformation { get; set; }
    }
}
