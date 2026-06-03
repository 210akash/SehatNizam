using MediatR;

namespace ERP.Mediator.Mediator.Referrer.Query
{
    public class DeleteReferrerQuery : IRequest<bool>
    {
        public DeleteReferrerQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}