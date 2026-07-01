using System.Collections.Generic;
namespace ERP.BusinessModels.ResponseVM
{
    public class GetService
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public long? DepartmentId { get; set; }
        public long ServiceTypeId { get; set; }
        public GetServiceType ServiceType { get; set; }
        public GetDepartment Department { get; set; }
        public bool IsActive { get; set; }
        public bool? IsSurgical { get; set; }
    }
}
