namespace ERP.Entities.Models
{
    public class LabOrder : BaseEntity
    {
        public long AppointmentId { get; set; }
        public long LabOrderTypeId { get; set; }
        public string ClinicalNotes { get; set; }
        public Status Status { get; set; }
        public long StatusId { get; set; }
        public Appointment Appointment { get; set; }
        public LabOrderType LabOrderType { get; set; }
    }
}
