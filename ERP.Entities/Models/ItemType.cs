using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class ItemType : BaseEntity
    {
        [MaxLength(7)]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public long SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }
    }
}
