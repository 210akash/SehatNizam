namespace ERP.BusinessModels.ResponseVM
{
    public class GetVendor
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public string NTN { get; set; }
        public string TermsConditions { get; set; }
        public int CreditDays { get; set; }
        public long VendorTypeId { get; set; }
        public GetVendorType VendorType { get; set; }

        public GetUser CreatedBy { get; set; }
        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
    }
}
