using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Shop : BaseEntity
    {
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string SecondaryPhoneNo { get; set; }
        public string PinLocation { get; set; }
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }
        public bool? IsVerified { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public bool? IsTagFromMob { get; set; } = false;

        public long TerritoryId { get; set; }
        public virtual Territory Territory { get; set; }

        public long? SchedulerId { get; set; }
        public virtual Scheduler Scheduler { get; set; }

        public Guid? VerifiedById { get; set; }
        public AspNetUsers VerifiedBy { get; set; }

        public long ShopTypeId { get; set; }
        public ShopType ShopType { get; set; }

        public AspNetUsers CreatedBy { get; set; }

        public long? StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }
        public virtual ICollection<Attachments> Attachments { get; set; }
        public virtual ICollection<RouteShop> RouteShop { get; set; }
        public virtual ICollection<ShopRouteFrequency> ShopRouteFrequency { get; set; }

        public int? PepsiFridge { get; set; }
        public int? CokeFridge { get; set; }
        public int? NestleFridge { get; set; }
        public int? NesfrutaFridge { get; set; }
        public int? OthersFridge { get; set; }
        public string Landmark { get; set; }
    }
}
