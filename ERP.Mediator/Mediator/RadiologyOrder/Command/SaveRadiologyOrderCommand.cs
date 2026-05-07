using MediatR;

namespace ERP.Mediator.Mediator.RadiologyOrder.Command
{
    public class SaveRadiologyOrderCommand : IRequest<int>
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public long RadiologyTypeId { get; set; }
        public string ClinicalNotes { get; set; }
        public long StatusId { get; set; }
    }
}
