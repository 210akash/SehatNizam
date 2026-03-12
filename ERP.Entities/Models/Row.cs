using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Row : BaseEntity
    {
        public string Name { get; set; }

        public long RackId { get; set; }
        public virtual Rack Rack { get; set; }
    }
}
