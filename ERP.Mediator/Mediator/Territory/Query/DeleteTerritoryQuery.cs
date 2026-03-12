using MediatR;

namespace ERP.Mediator.Mediator.Territory.Query
{
    public class DeleteTerritoryQuery : IRequest<long>
    {
        public DeleteTerritoryQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}