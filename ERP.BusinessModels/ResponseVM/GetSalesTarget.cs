using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetSalesTarget
    {
        public long Id { get; set; }
        public long Target { get; set; }
        public long AchievedTarget { get; set; }
        public DateTime TargetMonth { get; set; }
        public DateTime? CreatedDate { get; set; }

        //public long? ZoneId { get; set; }
        //public GetZone Zone { get; set; }

        //public long? TerritoryId { get; set; }
        //public GetTerritory Territory { get; set; }

        public Guid? UserId { get; set; }
        public GetUsers User { get; set; }
    }
}
