using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Attachments : BaseEntity
    {
        [MaxLength(100)]
        public string Name { get; set; }
        public string ImageName { get; set; }

        public long? DealershipId { get; set; }
        public virtual Dealership Dealership { get; set; }

        public long? ShopId { get; set; }
        public virtual Shop Shop { get; set; }

        //public long? ProductId { get; set; }
        //public virtual Product Product { get; set; }

        public Guid? UserId { get; set; }
        public virtual AspNetUsers User { get; set; }

        public long? MarkShopVisitId { get; set; }
        public virtual MarkShopVisit MarkShopVisit { get; set; }

        public long? OrderId { get; set; }
        public virtual Order Order { get; set; }

        public bool? IsInspectionImage { get; set; }

        public long? InterviewId { get; set; }
        public virtual Interview Interview { get; set; }
    }
}
