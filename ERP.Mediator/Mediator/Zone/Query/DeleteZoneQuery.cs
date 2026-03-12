using MediatR;

namespace ERP.Mediator.Mediator.Zone.Query
{
    public class DeleteZoneQuery : IRequest<long>
    {
        public DeleteZoneQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}