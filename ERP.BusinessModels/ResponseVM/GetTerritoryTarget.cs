using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetTerritoryTarget
    {
        public GetSalesTarget TerritoryTarget { get; set; }
        public List<GetSalesTarget> Target { get; set; }
    }
}
