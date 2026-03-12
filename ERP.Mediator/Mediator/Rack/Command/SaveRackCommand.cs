using MediatR;

namespace ERP.Mediator.Mediator.Rack.Command
{
    public class SaveRackCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
