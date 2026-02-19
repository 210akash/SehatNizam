using MediatR;

namespace ERP.Mediator.Mediator.GST.Query
{
    public class DeleteGSTQuery : IRequest<bool>
    {
        public DeleteGSTQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}