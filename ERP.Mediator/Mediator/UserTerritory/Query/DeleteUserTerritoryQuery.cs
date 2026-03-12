using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Query
{
    public class DeleteUserTerritoryQuery : IRequest<long>
    {
        public DeleteUserTerritoryQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}