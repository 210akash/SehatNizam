namespace ERP.Entities.Models
{
    public class LabOrderType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CustomFieldsSchema { get; set; }
        public long ServiceId { get; set; }   // 🔥 ADD THIS
        public Service Service { get; set; }
    }
}
