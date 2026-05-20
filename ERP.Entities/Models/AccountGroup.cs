using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class AccountGroup : BaseEntity
    {
        [MaxLength(14)]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Opening { get; set; }
        public decimal CreditLimit { get; set; }

        public long AccountId { get; set; }
        public virtual Account Account { get; set; }

        public long? VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        public long? DealershipId { get; set; }
        public virtual Dealership Dealership { get; set; }

        public virtual AspNetUsers Employee { get; set; }
        public Guid? EmployeeId { get; set; }

        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
