using MediatR;
using System.Collections.Generic;
namespace ERP.Mediator.Mediator.Appointment.Command
{
    public class SaveAppointmentAttachmentCommand : IRequest<long>
    {
        public long AppointmentId { get; set; }
        public List<SaveAppointmentAttachmentFiles> Files { get; set; }
    }

    public class SaveAppointmentAttachmentFiles
    {
        public string ImageName { get; set; }
        public string FileSource { get; set; }
        public string Extension { get; set; }
    }


}