using MediatR;

namespace ERP.Mediator.Mediator.TriageCategory.Query
{
    public class DeleteTriageCategoryQuery : IRequest<bool>
    {
        public DeleteTriageCategoryQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}