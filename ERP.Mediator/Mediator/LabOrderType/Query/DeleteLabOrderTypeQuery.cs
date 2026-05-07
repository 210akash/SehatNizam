using MediatR;

namespace ERP.Mediator.Mediator.LabOrderType.Query
{
    public class DeleteLabOrderTypeQuery : IRequest<bool>
    {
        public DeleteLabOrderTypeQuery(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
