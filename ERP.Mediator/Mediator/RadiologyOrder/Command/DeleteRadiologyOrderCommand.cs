using MediatR;

namespace ERP.Mediator.Mediator.RadiologyOrder.Command
{
    public class DeleteRadiologyOrderCommand : IRequest<bool>
    {
        public long Id { get; set; }

        public DeleteRadiologyOrderCommand(long id)
        {
            Id = id;
        }
    }
}
