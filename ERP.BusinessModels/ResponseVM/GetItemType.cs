namespace ERP.BusinessModels.ResponseVM
{
    public class GetItemType
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long SubCategoryId { get; set; }
        public GetSubCategory SubCategory { get; set; }

        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
