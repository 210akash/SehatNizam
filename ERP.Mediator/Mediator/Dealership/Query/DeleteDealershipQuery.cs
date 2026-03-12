using MediatR;

namespace ERP.Mediator.Mediator.Dealership.Query
{
    public class DeleteDealershipQuery : IRequest<long>
    {
        public DeleteDealershipQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}