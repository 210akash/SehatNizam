using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetAccountGroup
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Opening { get; set; }
        public decimal CreditLimit { get; set; }
        public long AccountId { get; set; }
        public GetAccount Account { get; set; }

        public long? VendorId { get; set; }
        public virtual GetVendor Vendor { get; set; }

        public long? DealershipId { get; set; }
        public virtual GetDealership Dealership { get; set; }

        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
