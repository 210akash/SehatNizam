namespace ERP.Entities.Models
{
    public class RadiologyOrder : BaseEntity
    {
        public long? AppointmentId { get; set; }
        public long RadiologyTypeId { get; set; }
        public string ClinicalNotes { get; set; }
        public long StatusId { get; set; }
        public AppointmentStatus Status { get; set; }
        public Appointment Appointment { get; set; }
        public RadiologyType RadiologyType { get; set; }
        public RadiologyStudyResult RadiologyStudyResult { get; set; }
    }
}
