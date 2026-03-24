using MediatR;

namespace ERP.Mediator.Mediator.VisitType.Query
{
    public class DeleteVisitTypeQuery : IRequest<bool>
    {
        public DeleteVisitTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}