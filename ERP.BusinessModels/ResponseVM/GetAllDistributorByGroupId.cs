using ERP.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetAllDistributorByGroupId
    {
        public long? DealershipId { get; set; }
        public string DealershipName { get; set; }
        public string TerritoryName { get; set; }
        public string AreaName { get; set; }
        public string ZoneName { get; set; }
        public string RegionName { get; set; }
        public bool IsSelected { get; set; }
        public bool IsOccupiedInOtherGroup { get; set; }
        public string GroupName { get; set; }
    }
}
