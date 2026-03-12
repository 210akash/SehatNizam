using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Entities.Models
{
    public class CancelDispatchDetail : BaseEntity
    {
        public long CancelDispatchId { get; set; }
        public virtual CancelDispatch CancelDispatch { get; set; }
        public long OrderItemId { get; set; }
        public virtual OrderItems OrderItem { get; set; }

        public long Quantity { get; set; }
    }
}
