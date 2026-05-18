using MediatR;

namespace ERP.Mediator.Mediator.ServiceType.Command
{
    public class DeleteServiceTypeCommand : IRequest<bool>
    {
        public long Id { get; set; }

        public DeleteServiceTypeCommand(long id)
        {
            Id = id;
        }
    }
}
