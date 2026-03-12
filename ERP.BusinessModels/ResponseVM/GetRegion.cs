using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetRegion
    {
        public bool IsActive { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long? CompanyId { get; set; }
        public virtual GetCompany Company { get; set; }

        public List<GetZone> Zone { get; set; }
    }

    public class GetRegionLite
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Coordinates { get; set; }
    }
}
