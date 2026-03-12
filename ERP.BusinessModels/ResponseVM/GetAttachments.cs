using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetAttachments
    {
        public long Id { get; set; }
        public string ImageName { get; set; }
        public string FileSource { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }

        public long? DealershipId { get; set; }
        public GetDealership Dealership { get; set; }

        public long? ShopId { get; set; }
        public GetShop Shop { get; set; }

        public long? OrderId { get; set; }
        public GetOrder Order { get; set; }

        //public long? VisitPlannerId { get; set; }
        //public GetVisitPlanner VisitPlanner { get; set; }
    }

    public class GetDealershipAttachments
    {
        public string ImageName { get; set; }
        public string FileSource { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
