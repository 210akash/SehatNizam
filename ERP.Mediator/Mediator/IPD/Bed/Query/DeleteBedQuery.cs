using MediatR;

namespace ERP.Mediator.Mediator.IPD.Bed.Query
{
    public class DeleteBedQuery : IRequest<bool>
    {
        public DeleteBedQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}