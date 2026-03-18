
namespace ERP.Entities.Models
{
    public class TriagePriority : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public long CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
