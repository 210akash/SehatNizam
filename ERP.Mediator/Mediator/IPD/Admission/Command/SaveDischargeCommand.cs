using ERP.Mediator.Mediator.Appointment.Command;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.IPD.Admission.Command
{
    public class SaveDischargeCommand : IRequest<long>
    {
        public long AdmissionId { get; set; }
        public DateTime DischargeDate { get; set; }
        public string DischargeSummary { get; set; }
        public string FollowUpInstructions { get; set; }
        public long AppointmentId { get; set; }
        public List<SaveAppointmentAttachmentFiles> Files { get; set; }
    }
}