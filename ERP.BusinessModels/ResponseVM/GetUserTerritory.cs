using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetUserTerritory
    {
        public long Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public bool? IsAllTerritoryCheck { get; set; }

        public Guid? UserId { get; set; }
        public GetUsers User { get; set; }

        public long? TerritoryId { get; set; }
        public GetTerritory Territory { get; set; }

        public long? ZoneId { get; set; }
        public GetZone Zone { get; set; }

        public long? RegionId { get; set; }
        public GetRegion Region { get; set; }

        public long? AreaId { get; set; }
        public GetArea Area { get; set; }

        public long? ShopId { get; set; }
        public GetShop Shop { get; set; }
    }
}
