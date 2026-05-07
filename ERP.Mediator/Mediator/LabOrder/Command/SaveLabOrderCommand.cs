using MediatR;

namespace ERP.Mediator.Mediator.LabOrder.Command
{
    public class SaveLabOrderCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public long LabOrderTypeId { get; set; }
        public long StatusId { get; set; }
    }
}
