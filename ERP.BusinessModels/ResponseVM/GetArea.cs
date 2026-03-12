using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetArea
    {
        public bool IsActive { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long? ZoneId { get; set; }
        public GetZone Zone { get; set; }

        public List<GetTerritory> Territory { get; set; }
    }

    public class GetAreaLite
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Coordinates { get; set; }
    }
}
