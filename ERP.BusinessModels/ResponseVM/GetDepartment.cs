namespace ERP.BusinessModels.ResponseVM
{
    public class GetDepartment
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Clinical { get; set; }
        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
        public GetCreatedBy CreatedBy { get; set; }
    }
}
