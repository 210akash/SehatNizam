using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetShop
    {
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string PinLocation { get; set; }
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }
        public bool? IsVerified { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public bool? IsTagFromMob { get; set; } = false;
        public long? Sequence { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long TerritoryId { get; set; }
        public GetTerritory Territory { get; set; }

        //public long? SchedulerId { get; set; }
        //public GetScheduler Scheduler { get; set; }

        public Guid? VerifiedById { get; set; }
        public GetUsers VerifiedBy { get; set; }

        public Guid? CreatedById { get; set; }
        public GetCreatedBy CreatedBy { get; set; }

        public long? StatusId { get; set; }
        public virtual GetStatus Status { get; set; }
        public string Remarks { get; set; }
        public long? ShopTypeId { get; set; }

        public List<GetAttachments> Attachments { get; set; }
        public List<GetRouteShop> RouteShop { get; set; }
        public List<GetShopRouteFrequency> ShopRouteFrequency { get; set; }

        public string SecondaryPhoneNo { get; set; }
        public int? PepsiFridge { get; set; }
        public int? CokeFridge { get; set; }
        public int? NestleFridge { get; set; }
        public int? NesfrutaFridge { get; set; }
        public int? OthersFridge { get; set; }
        public string Landmark { get; set; }
    }

    public class GetShopLite
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string PinLocation { get; set; }
        public string TerritoryName { get; set; }
        public string ZoneName { get; set; }
        public string AreaName { get; set; }
        public string RegionName { get; set; }
        public string DistributorName { get; set; }
        public string Image { get; set; }
    }

    public class GetShopBasic
    {
        public long Id { get; set; }
        public long DealershipId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
