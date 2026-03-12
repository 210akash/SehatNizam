namespace ERP.BusinessModels.ResponseVM
{
    public class GetSubCategory
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; }
        public GetCategory Category { get; set; }

        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
