namespace ERP.Entities.Models
{
    public class LabOrder
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public long LabOrderTypeId { get; set; }
        public Status Status { get; set; }
        public long StatusId { get; set; }
        public Appointment Appointment { get; set; }
        public LabOrderType LabOrderType { get; set; }
    }
}
