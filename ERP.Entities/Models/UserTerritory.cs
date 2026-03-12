using System;

namespace ERP.Entities.Models
{
    public class UserTerritory : BaseEntity
    {
        public bool? IsAllTerritoryCheck { get; set; }

        public Guid? UserId { get; set; }
        public virtual AspNetUsers User { get; set; }

        public long? RegionId { get; set; }
        public virtual Region Region { get; set; }

        public long? ZoneId { get; set; }
        public virtual Zone Zone { get; set; }

        public long? AreaId { get; set; }
        public virtual Area Area { get; set; }

        public long? TerritoryId { get; set; }
        public virtual Territory Territory { get; set; }

        public long? ShopId { get; set; }
        public virtual Shop Shop { get; set; }
    }
}


