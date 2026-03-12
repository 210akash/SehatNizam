using MediatR;

namespace ERP.Mediator.Mediator.Region.Query
{
    public class DeleteRegionQuery : IRequest<long>
    {
        public DeleteRegionQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}