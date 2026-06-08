using MediatR;

namespace ERP.Mediator.Mediator.IPD.Ward.Query
{
    public class DeleteWardQuery : IRequest<bool>
    {
        public DeleteWardQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}