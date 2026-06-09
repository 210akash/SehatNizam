namespace ERP.Entities.Models
{
    public class AdmissionBed : BaseEntity
    {
        public long AdmissionId { get; set; }
        public Admission Admission { get; set; }
        public long BedId { get; set; }
        public Bed Bed { get; set; }
    }
}
