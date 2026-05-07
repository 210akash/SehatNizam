using MediatR;

namespace ERP.Mediator.Mediator.RadiologyType.Command
{
    public class DeleteRadiologyTypeCommand : IRequest<bool>
    {
        public long Id { get; set; }

        public DeleteRadiologyTypeCommand(long id)
        {
            Id = id;
        }
    }
}
