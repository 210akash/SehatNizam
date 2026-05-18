using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetService
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public long? DepartmentId { get; set; }
        public long ServiceTypeId { get; set; }
        public GetServiceType ServiceType { get; set; }
        public string DepartmentName { get; set; }
        public bool IsActive { get; set; }
    }
}
