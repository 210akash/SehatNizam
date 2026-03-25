namespace ERP.Entities.Models
{
    public class AppointmentAttachment : BaseEntity
    {
        public long AppointmentId { get; set; }
        public string Attachment { get; set; }
        public Appointment Appointment { get; set; }
    }
}
