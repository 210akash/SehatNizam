using MediatR;

namespace ERP.Mediator.Mediator.ServiceType.Command
{
    public class SaveServiceTypeCommand : IRequest<int>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
