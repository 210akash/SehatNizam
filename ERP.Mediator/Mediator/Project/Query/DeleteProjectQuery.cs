using MediatR;

namespace ERP.Mediator.Mediator.Project.Query
{
    public class DeleteProjectQuery : IRequest<bool>
    {
        public DeleteProjectQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}