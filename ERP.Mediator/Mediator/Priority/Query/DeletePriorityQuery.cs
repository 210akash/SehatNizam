using MediatR;

namespace ERP.Mediator.Mediator.Priority.Query
{
    public class DeletePriorityQuery : IRequest<bool>
    {
        public DeletePriorityQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}