using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Account : BaseEntity
    {
        [MaxLength(10)]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Opening { get; set; }

        public long? AccountFlowId { get; set; }
        public virtual AccountFlow AccountFlow { get; set; }

        public long AccountTypeId { get; set; }
        public virtual AccountType AccountType { get; set; }

        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
