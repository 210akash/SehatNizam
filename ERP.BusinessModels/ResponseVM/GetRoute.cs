using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetRoute
    {
        public long Id { get; set; }
        public string Name { get; set; }
        //public string VisitDay { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long TerritoryId { get; set; }
        public GetTerritory Territory { get; set; }

        public List<GetRouteShop> RouteShop { get; set; }
        public List<GetShopRouteFrequency> ShopRouteFrequency { get; set; }
    }
}
