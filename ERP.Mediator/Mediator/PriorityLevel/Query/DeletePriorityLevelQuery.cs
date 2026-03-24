using MediatR;

namespace ERP.Mediator.Mediator.PriorityLevel.Query
{
    public class DeletePriorityLevelQuery : IRequest<bool>
    {
        public DeletePriorityLevelQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}