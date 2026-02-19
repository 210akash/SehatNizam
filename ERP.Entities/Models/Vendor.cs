using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Vendor : BaseEntity
    {
        [MaxLength(5)]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string NTN { get; set; }
        public string Email { get; set; }
        public string TermsConditions { get; set; }
        public int CreditDays { get; set; }
        
        public long? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public long? VendorTypeId { get; set; }
        public virtual VendorType VendorType { get; set; }
    }
}
