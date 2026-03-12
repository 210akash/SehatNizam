using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetTerritory
    {
        public bool IsActive { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
        public string SaleModel { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long AreaId { get; set; }
        public GetArea Area { get; set; }

        public List<GetDealership> Dealership { get; set; }
        public List<GetShop> Shop { get; set; }
    }

    public class GetTerritoryLite
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Coordinates { get; set; }
    }
}
