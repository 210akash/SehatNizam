using MediatR;

namespace ERP.Mediator.Mediator.Rack.Query
{
    public class DeleteRackQuery : IRequest<long>
    {
        public DeleteRackQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}