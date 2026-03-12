using MediatR;

namespace ERP.Mediator.Mediator.Row.Query
{
    public class DeleteRowQuery : IRequest<long>
    {
        public DeleteRowQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}