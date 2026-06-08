using System.ComponentModel.DataAnnotations;
namespace ERP.Entities.Models
{
    public class Ward : BaseEntity
    {
        [MaxLength(2)]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }
    }
}
