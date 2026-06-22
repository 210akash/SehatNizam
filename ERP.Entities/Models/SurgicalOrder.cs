using System;

namespace ERP.Entities.Models
{
    public class SurgicalOrder : BaseEntity
    {
        public long AppointmentId { get; set; }
        public long ServiceId { get; set; }
        public Guid SurgeonId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public long StatusId { get; set; }
        public string ClinicalNotes { get; set; }
        public DateTime? CompletedDateTime { get; set; }
        public DateTime? CancelledDateTime { get; set; }
        public string CancellationReason { get; set; }

        public Appointment Appointment { get; set; }
        public Service Service { get; set; }
        public AspNetUsers Surgeon { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
