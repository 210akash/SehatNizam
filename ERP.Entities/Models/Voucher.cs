using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Voucher : BaseEntity
    {
        [MaxLength(8)]
        public string Code { get; set; }

        public long TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }

        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
