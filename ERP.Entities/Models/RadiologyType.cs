namespace ERP.Entities.Models
{
    public class RadiologyType : BaseEntity
    {
        public string Name { get; set; }
        public Company Company { get; set; }
        public long CompanyId { get; set; }
    }
}
