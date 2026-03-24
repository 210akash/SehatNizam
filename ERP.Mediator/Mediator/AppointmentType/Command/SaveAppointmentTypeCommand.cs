using MediatR;

namespace ERP.Mediator.Mediator.AppointmentType.Command
{
    public class SaveAppointmentTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
