using System.ComponentModel.DataAnnotations;
namespace ERP.Entities.Models
{
    public class AccountCategory : BaseEntity
    {
        [MaxLength(2)]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? AccountHeadId { get; set; }
        public virtual AccountHead AccountHead { get; set; }
        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
