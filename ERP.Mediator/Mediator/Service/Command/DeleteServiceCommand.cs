using MediatR;

namespace ERP.Mediator.Mediator.Service.Command
{
    public class DeleteServiceCommand : IRequest<bool>
    {
        public long Id { get; set; }

        public DeleteServiceCommand(long id)
        {
            Id = id;
        }
    }
}
