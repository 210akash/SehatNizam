using MediatR;

namespace ERP.Mediator.Mediator.Route.Query
{
    public class DeleteRouteQuery : IRequest<long>
    {
        public DeleteRouteQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}