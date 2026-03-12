using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class EmployeeLeaveType : BaseEntity
    {
        [MaxLength(6)]
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
