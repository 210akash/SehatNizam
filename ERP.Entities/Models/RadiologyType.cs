namespace ERP.Entities.Models
{
    public class RadiologyType : BaseEntity
    {
        public string Name { get; set; }
        public long ServiceId { get; set; }   // 🔥 ADD THIS
        public Service Service { get; set; }
        public Company Company { get; set; }
        public long CompanyId { get; set; }
    }
}
