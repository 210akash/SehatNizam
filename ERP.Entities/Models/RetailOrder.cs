using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class RetailOrder : BaseEntity
    {
        public long? ShopId { get; set; }
        public virtual Shop Shop { get; set; }

        public long RetailOrderStatusId { get; set; } 
        public virtual Status RetailOrderStatus { get; set; }

        public string Comments { get; set; }
        public string Reference { get; set; }
        public string Department { get; set; }

        public virtual ICollection<RetailOrderItems> RetailOrderItems { get; set; } 
        public virtual ICollection<RetailOrderProcess> RetailOrderProcess { get; set; }
    }
}
