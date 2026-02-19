using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class SubCategory : BaseEntity
    {
        [MaxLength(4)]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public long CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
