using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetZone
    {
        public bool IsActive { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long? RegionId { get; set; }
        public GetRegion Region { get; set; }

        public List<GetUsers> Salesmen { get; set; }
        public List<GetArea> Area { get; set; }
    }

    public class GetZoneLite
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Coordinates { get; set; }
    }
}
