using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Service : BaseEntity
    {
        [MaxLength(10)]
        public string Code { get; set; }
        public string Name { get; set; }           // ECG, X-Ray, Procedure
        public decimal BasePrice { get; set; }
        public long? DepartmentId { get; set; }
        public Department Department { get; set; }
        public long ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}
