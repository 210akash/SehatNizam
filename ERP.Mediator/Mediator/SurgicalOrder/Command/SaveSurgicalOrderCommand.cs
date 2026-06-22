using MediatR;
using System;

namespace ERP.Mediator.Mediator.SurgicalOrder.Command
{
    public class SaveSurgicalOrderCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public long ServiceId { get; set; }
        public Guid SurgeonId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public long StatusId { get; set; }
        public string ClinicalNotes { get; set; }
        public DateTime? CompletedDateTime { get; set; }
        public DateTime? CancelledDateTime { get; set; }
        public string CancellationReason { get; set; }
    }
}
