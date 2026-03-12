using MediatR;

namespace ERP.Mediator.Mediator.Area.Query
{
    public class DeleteAreaQuery : IRequest<long>
    {
        public DeleteAreaQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}