using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class ShopOrderReturn : BaseEntity
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long ShopOrderId { get; set; }
        public virtual ShopOrder ShopOrder { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }

        public virtual List<ShopOrderReturnDetail> ShopOrderReturnDetail { get; set; }
    }
}
