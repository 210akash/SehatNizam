using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetDSFRoute
    {
        public long RouteId { get; set; }
        public GetRoute Route { get; set; }

        public Guid DSFId { get; set; }
        public GetUsers DSF { get; set; }

        public bool? IsActive { get; set; }
    }
}
