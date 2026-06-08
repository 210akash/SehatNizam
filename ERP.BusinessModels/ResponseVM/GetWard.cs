namespace ERP.BusinessModels.ResponseVM
{
    public class GetWard
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long DepartmentId { get; set; }
        public GetDepartment Department { get; set; }
        public long ProjectId { get; set; }
        public GetProject Project { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
