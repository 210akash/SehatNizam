using System;
using System.Collections;

namespace ERP.Entities.Models
{
    public class DSFRoute : BaseEntity
    {
        public long RouteId { get; set; }
        public virtual Route Route { get; set; }

        public Guid? DSFId { get; set; }
        public virtual AspNetUsers DSF { get; set; }
    }
}
