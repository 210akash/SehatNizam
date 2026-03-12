namespace ERP.Entities.Models
{
    public class VendorType : BaseEntity
    {
        public string Name { get; set; }
        public long? CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
