using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Entities.Models
{
    public class CancelDispatch : BaseEntity
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long OrderId { get; set; }
        public virtual Order Order { get; set; }

        public long? StatusId { get; set; }
        public virtual Status Status { get; set; }

        public virtual List<CancelDispatchDetail> CancelDispatchDetail { get; set; }
        public virtual List<OrderProcess> OrderProcess { get; set; }
    }
}
