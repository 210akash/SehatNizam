using MediatR;

namespace ERP.Mediator.Mediator.IGPType.Command
{
    public class SaveIGPTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
