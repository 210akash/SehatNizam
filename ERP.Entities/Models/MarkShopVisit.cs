using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class MarkShopVisit : BaseEntity
    {
        public Guid DSFId { get; set; }
        public virtual AspNetUsers DSF { get; set; }

        public long ShopId { get; set; }
        public virtual Shop Shop { get; set; }

        public bool IsOpen { get; set; } = false;
        public string Comments { get; set; }
        public string PinLocation { get; set; }

        public ICollection<Attachments> Attachments { get; set; }
    }
}
