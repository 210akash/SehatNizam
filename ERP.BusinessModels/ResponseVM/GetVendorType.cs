namespace ERP.BusinessModels.ResponseVM
{
    public class GetVendorType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CompanyId { get; set; }

        public GetUser CreatedBy { get; set; }
        public GetCompany Company { get; set; }
    }
}
