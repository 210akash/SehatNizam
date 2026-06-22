using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetSurgicalOrder
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public GetAppointment Appointment { get; set; }
        public long ServiceId { get; set; }
        public GetService Service { get; set; }
        public Guid SurgeonId { get; set; }
        public GetCreatedBy Surgeon { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public long StatusId { get; set; }
        public GetAppointmentStatus Status { get; set; }
        public string ClinicalNotes { get; set; }
        public DateTime? CompletedDateTime { get; set; }
        public DateTime? CancelledDateTime { get; set; }
        public string CancellationReason { get; set; }
    }
}
